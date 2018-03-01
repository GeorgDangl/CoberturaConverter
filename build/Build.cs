﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Core;
using System.Xml.XPath;
using Nuke.Common.Tools.DocFx;
using Nuke.Core.Tooling;
using Nuke.Core.Utilities;
using Nuke.Core.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Core.IO.FileSystemTasks;
using static Nuke.Core.IO.PathConstruction;
using static Nuke.Core.EnvironmentInfo;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.GitHub.ChangeLogExtensions;
using Nuke.GitHub;
using Nuke.WebDocu;
using static Nuke.Common.Tools.DocFx.DocFxTasks;
using static Nuke.WebDocu.WebDocuTasks;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using Nuke.Common.Tools.OpenCover;
using static Nuke.Common.Tools.OpenCover.OpenCoverTasks;
using static Nuke.CodeGeneration.CodeGenerator;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main () => Execute<Build>(x => x.Compile);

    // Auto-injection fields:

    [GitVersion] readonly GitVersion GitVersion;
    // Semantic versioning. Must have 'GitVersion.CommandLine' referenced.

    [GitRepository] readonly GitRepository GitRepository;
    // Parses origin, branch name and head from git config.

    [Parameter] string MyGetSource;
    [Parameter] string MyGetApiKey;
    [Parameter] string DocuApiKey;
    [Parameter] string DocuApiEndpoint;
    [Parameter] string GitHubAuthenticationToken;

    string DocFxFile => SolutionDirectory / "docfx.json";
    string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    Target Clean => _ => _
            .Executes(() =>
            {
                DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
                EnsureCleanDirectory(OutputDirectory);
            });

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                DotNetRestore(s => DefaultDotNetRestore);
            });

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                DotNetBuild(s => DefaultDotNetBuild
                    .SetFileVersion(GitVersion.AssemblySemVer));
            });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile)
                .EscapeStringPropertyForMsBuild();
            DotNetPack(s => DefaultDotNetPack
                .SetPackageReleaseNotes(changeLog));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = GlobFiles(SolutionDirectory / "test", "*.csproj");
            var testRun = 1;
            foreach (var testProject in testProjects)
            {
                var projectDirectory = Path.GetDirectoryName(testProject);
                var dotnetXunitSettings = new DotNetSettings()
                    // Need to set it here, otherwise it takes the one from NUKEs .tmp directory
                    .SetToolPath(ToolPathResolver.GetPathExecutable("dotnet"))
                    .SetWorkingDirectory(projectDirectory)
                    .SetArgumentConfigurator(c => c.Add("xunit")
                        .Add("-nobuild")
                        .Add("-xml {value}", "\"" + OutputDirectory / $"test_{testRun++}.testresults" + "\""));
                ProcessTasks.StartProcess(dotnetXunitSettings)
                    .AssertZeroExitCode();
            }

            PrependFrameworkToTestresults();
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = GlobFiles(SolutionDirectory / "test", "*.csproj").ToList();
            var dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");
            for (var i = 0; i < testProjects.Count; i++)
            {
                var testProject = testProjects[i];

                var projectDirectory = Path.GetDirectoryName(testProject);
                var snapshotIndex = i;
                var toolSettings = new ToolSettings()
                    .SetToolPath(ToolPathResolver.GetPackageExecutable("JetBrains.dotCover.CommandLineTools", "tools/dotCover.exe"))
                    .SetArgumentConfigurator(a => a
                        .Add("cover")
                        .Add($"/TargetExecutable=\"{dotnetPath}\"")
                        .Add($"/TargetWorkingDir=\"{projectDirectory}\"")
                        .Add("/TargetArguments=\"xunit -nobuild\"")
                        .Add("/Filters=\"+:CoberturaConverter;+:Nuke.CoberturaConverter;+:CoberturaConverter.CommandLine\"")
                        .Add("/AttributeFilters=\"System.CodeDom.Compiler.GeneratedCodeAttribute\"")
                        .Add($"/Output=\"{OutputDirectory / $"coverage{snapshotIndex:00}.snapshot"}\""));
                ProcessTasks.StartProcess(toolSettings)
                    .AssertZeroExitCode();
            }

            var snapshots = testProjects.Select((t, i) => OutputDirectory / $"coverage{i:00}.snapshot")
                .Select(p => p.ToString())
                .Aggregate((c, n) => c + ";" + n);

            var mergeSettings = new ToolSettings()
                .SetToolPath(ToolPathResolver.GetPackageExecutable("JetBrains.dotCover.CommandLineTools", "tools/dotCover.exe"))
                .SetArgumentConfigurator(a => a
                    .Add("merge")
                    .Add($"/Source=\"{snapshots}\"")
                    .Add($"/Output=\"{OutputDirectory / "coverage.snapshot"}\""));
            ProcessTasks.StartProcess(mergeSettings)
                .AssertZeroExitCode();

            var reportSettings = new ToolSettings()
                .SetToolPath(ToolPathResolver.GetPackageExecutable("JetBrains.dotCover.CommandLineTools", "tools/dotCover.exe"))
                .SetArgumentConfigurator(a => a
                    .Add("report")
                    .Add($"/Source=\"{OutputDirectory / "coverage.snapshot"}\"")
                    .Add($"/Output=\"{OutputDirectory / "coverage.xml"}\"")
                    .Add("/ReportType=\"DetailedXML\""));
            ProcessTasks.StartProcess(reportSettings)
                .AssertZeroExitCode();

            var reportGeneratorSettings = new ToolSettings()
                .SetToolPath(ToolPathResolver.GetPackageExecutable("ReportGenerator", "tools/ReportGenerator.exe"))
                .SetArgumentConfigurator(a => a
                    .Add($"-reports:\"{OutputDirectory / "coverage.xml"}\"")
                    .Add($"-targetdir:\"{OutputDirectory / "CoverageReport"}\""));
            ProcessTasks.StartProcess(reportGeneratorSettings)
                .AssertZeroExitCode();
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => MyGetSource)
        .Requires(() => MyGetApiKey)
        .Requires(() => Configuration.EqualsOrdinalIgnoreCase("Release"))
        .Executes(() =>
        {
            GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(MyGetSource)
                        .SetApiKey(MyGetApiKey));
                });
        });

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if (IsLocalBuild)
            {
                SetVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional");
                SetVariable("VisualStudioVersion", "15.0");
            }

            DocFxMetadata(DocFxFile, s => s.SetLogLevel(DocFxLogLevel.Verbose));
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            // Using README.md as index.md
            if (File.Exists(SolutionDirectory / "index.md"))
            {
                File.Delete(SolutionDirectory / "index.md");
            }

            File.Copy(SolutionDirectory / "README.md", SolutionDirectory / "index.md");

            DocFxBuild(DocFxFile, s => s
                .ClearXRefMaps()
                .SetLogLevel(DocFxLogLevel.Verbose));

            File.Delete(SolutionDirectory / "index.md");
            Directory.Delete(SolutionDirectory / "api", true);
            Directory.Delete(SolutionDirectory / "obj", true);
        });

    Target UploadDocumentation => _ => _
        .DependsOn(Push) // To have a relation between pushed package version and published docs version
        .DependsOn(BuildDocumentation)
        .Requires(() => DocuApiKey)
        .Requires(() => DocuApiEndpoint)
        .Executes(() =>
        {
            WebDocu(s => s
                .SetDocuApiEndpoint(DocuApiEndpoint)
                .SetDocuApiKey(DocuApiKey)
                .SetSourceDirectory(OutputDirectory)
                .SetVersion(GitVersion.NuGetVersion)
            );
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhen(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes<Task>(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
            var nuGetPackages = GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray();

            await PublishRelease(new GitHubReleaseSettings()
                .SetArtifactPaths(nuGetPackages)
                .SetCommitSha(GitVersion.Sha)
                .SetReleaseNotes(completeChangeLog)
                .SetRepositoryName(repositoryInfo.repositoryName)
                .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                .SetTag(releaseTag)
                .SetToken(GitHubAuthenticationToken));
        });

    Target Generate => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            GenerateCode(
                metadataDirectory: RootDirectory / "src" / "Nuke.CoberturaConverter",
                generationBaseDirectory: RootDirectory / "src" / "Nuke.CoberturaConverter",
                baseNamespace: "Nuke.CoberturaConverter"
            );
        });

    void PrependFrameworkToTestresults()
    {
        var testResults = GlobFiles(OutputDirectory, "*.testresults");
        foreach (var testResultFile in testResults)
        {
            var frameworkName = GetFrameworkNameFromFilename(testResultFile);
            var xDoc = XDocument.Load(testResultFile);

            foreach (var testType in ((IEnumerable)xDoc.XPathEvaluate("//test/@type")).OfType<XAttribute>())
            {
                testType.Value = frameworkName + "+" + testType.Value;
            }

            foreach (var testName in ((IEnumerable)xDoc.XPathEvaluate("//test/@name")).OfType<XAttribute>())
            {
                testName.Value = frameworkName + "+" + testName.Value;
            }

            xDoc.Save(testResultFile);
        }
    }

    string GetFrameworkNameFromFilename(string filename)
    {
        var name = Path.GetFileName(filename);
        name = name.Substring(0, name.Length - ".testresults".Length);
        var startIndex = name.LastIndexOf('-');
        name = name.Substring(startIndex + 1);
        return name;
    }
}
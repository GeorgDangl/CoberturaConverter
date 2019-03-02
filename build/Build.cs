using Nuke.CoberturaConverter;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotCover;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using Nuke.WebDocu;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using static Nuke.CoberturaConverter.CoberturaConverterTasks;
using static Nuke.CodeGeneration.CodeGenerator;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.DocFX.DocFXTasks;
using Nuke.DocFX;
using static Nuke.Common.Tools.DotCover.DotCoverTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.GitHub.ChangeLogExtensions;
using static Nuke.GitHub.GitHubTasks;
using static Nuke.WebDocu.WebDocuTasks;
using Nuke.Azure.KeyVault;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Compile);

    [KeyVaultSettings(
        BaseUrlParameterName = nameof(KeyVaultBaseUrl),
        ClientIdParameterName = nameof(KeyVaultClientId),
        ClientSecretParameterName = nameof(KeyVaultClientSecret))]
    readonly KeyVaultSettings KeyVaultSettings;

    [Parameter] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] string KeyVaultBaseUrl;
    [Parameter] string KeyVaultClientId;
    [Parameter] string KeyVaultClientSecret;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [KeyVaultSecret] string DocuBaseUrl;
    [KeyVaultSecret] string GitHubAuthenticationToken;
    [KeyVaultSecret] string PublicMyGetSource;
    [KeyVaultSecret] string PublicMyGetApiKey;
    [KeyVaultSecret("CoberturaConverter-DocuApiKey")] string DocuApiKey;
    [KeyVaultSecret] string NuGetApiKey;

    string DocFxFile => RootDirectory / "docfx.json";

    string ChangeLogFile => RootDirectory / "CHANGELOG.md";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(x => x
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile)
                .EscapeStringPropertyForMsBuild();

            DotNetPack(x => x
                .SetConfiguration(Configuration)
                .SetPackageReleaseNotes(changeLog)
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory)
                .SetVersion(GitVersion.NuGetVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = GlobFiles(RootDirectory / "test", "*.csproj");
            var testRun = 1;
            foreach (var testProject in testProjects)
            {
                var projectDirectory = Path.GetDirectoryName(testProject);


                DotNetTest(x => x
                .SetNoBuild(true)
                .SetProjectFile(testProject)
                .SetTestAdapterPath(".")
                .SetLogger($"xunit;LogFilePath={OutputDirectory / $"test_{testRun++}.testresults.xml"}"));


                // TODO TEST FRAMEWORK
                throw new NotImplementedException();
            }

            PrependFrameworkToTestresults();
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = GlobFiles(RootDirectory / "test", "*.csproj").ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                var testProject = testProjects[i];
                var projectDirectory = Path.GetDirectoryName(testProject);
                // This is so that the global dotnet is used instead of the one that comes with NUKE
                var dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");
                var snapshotIndex = i;

                string xUnitOutputDirectory = OutputDirectory / $"test_{snapshotIndex:00}.testresults.xml";
                DotCoverCover(c => c
                    .SetTargetExecutable(dotnetPath)
                    .SetTargetWorkingDirectory(projectDirectory)
                    .SetTargetArguments($"xunit -nobuild -xml {xUnitOutputDirectory.DoubleQuoteIfNeeded()}")
                    .SetFilters("+:CoberturaConverter.Core")
                    .SetAttributeFilters("System.CodeDom.Compiler.GeneratedCodeAttribute")
                    .SetOutputFile(OutputDirectory / $"coverage{snapshotIndex:00}.snapshot"));
            }

            PrependFrameworkToTestresults();

            var snapshots = testProjects.Select((t, i) => OutputDirectory / $"coverage{i:00}.snapshot")
                .Select(p => p.ToString())
                .Aggregate((c, n) => c + ";" + n);

            DotCoverMerge(c => c
                .SetSource(snapshots)
                .SetOutputFile(OutputDirectory / "coverage.snapshot"));

            DotCoverReport(c => c
                .SetSource(OutputDirectory / "coverage.snapshot")
                .SetOutputFile(OutputDirectory / "coverage.xml")
                .SetReportType(DotCoverReportType.DetailedXml));

            // This is the report that's pretty and visualized in Jenkins
            ReportGenerator(c => c
                .SetReports(OutputDirectory / "coverage.xml")
                .SetTargetDirectory(OutputDirectory / "CoverageReport"));

            // This is the report in Cobertura format that integrates so nice in Jenkins
            // dashboard and allows to extract more metrics and set build health based
            // on coverage readings
            DotCoverToCobertura(s => s
                    .SetInputFile(OutputDirectory / "coverage.xml")
                    .SetOutputFile(OutputDirectory / "cobertura_coverage.xml"))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => PublicMyGetSource)
        .Requires(() => PublicMyGetApiKey)
        .Requires(() => NuGetApiKey)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            GlobFiles(OutputDirectory, "*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(PublicMyGetSource)
                        .SetApiKey(PublicMyGetApiKey));

                    if (GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
                    {
                        // Stable releases are published to NuGet
                        DotNetNuGetPush(s => s
                            .SetTargetPath(x)
                            .SetSource("https://api.nuget.org/v3/index.json")
                            .SetApiKey(NuGetApiKey));
                    }
                });
        });

    Target BuildDocFxMetadata => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DocFXMetadata(x => x.AddProjects(DocFxFile));
        });

    Target BuildDocumentation => _ => _
        .DependsOn(Clean)
        .DependsOn(BuildDocFxMetadata)
        .Executes(() =>
        {
            // Using README.md as index.md
            if (File.Exists(RootDirectory / "index.md"))
            {
                File.Delete(RootDirectory / "index.md");
            }

            File.Copy(RootDirectory / "README.md", RootDirectory / "index.md");

            DocFXBuild(x => x.SetConfigFile(DocFxFile));

            File.Delete(RootDirectory / "index.md");
            Directory.Delete(RootDirectory / "core", true);
            Directory.Delete(RootDirectory / "cli", true);
            Directory.Delete(RootDirectory / "nuke", true);
            Directory.Delete(RootDirectory / "obj", true);
        });

    Target UploadDocumentation => _ => _
        .DependsOn(Push) // To have a relation between pushed package version and published docs version
        .DependsOn(BuildDocumentation)
        .Requires(() => DocuApiKey)
        .Requires(() => DocuBaseUrl)
        .Executes(() =>
        {
            WebDocu(s => s
                .SetDocuBaseUrl(DocuBaseUrl)
                .SetDocuApiKey(DocuApiKey)
                .SetSourceDirectory(OutputDirectory)
                .SetVersion(GitVersion.NuGetVersion)
            );
        });

    Target PublishGitHubRelease => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubAuthenticationToken)
        .OnlyWhenDynamic(() => GitVersion.BranchName.Equals("master") || GitVersion.BranchName.Equals("origin/master"))
        .Executes<Task>(async () =>
        {
            var releaseTag = $"v{GitVersion.MajorMinorPatch}";

            var changeLogSectionEntries = ExtractChangelogSectionNotes(ChangeLogFile);
            var latestChangeLog = changeLogSectionEntries
                .Aggregate((c, n) => c + Environment.NewLine + n);
            var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;

            var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
            var nuGetPackages = GlobFiles(OutputDirectory, "*.nupkg").NotEmpty().ToArray();

            await PublishRelease(x => x
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
                specificationDirectory: RootDirectory / "src" / "Nuke.CoberturaConverter",
                namespaceProvider: x => "Nuke.CoberturaConverter",
                outputFileProvider: x => RootDirectory / "src" / "Nuke.CoberturaConverter" / "CoberturaConverterTasks.Generated.cs"
            );
        });

    void PrependFrameworkToTestresults()
    {
        var testResults = GlobFiles(OutputDirectory, "*.testresults*.xml");
        foreach (var testResultFile in testResults)
        {
            var frameworkName = GetFrameworkNameFromFilename(testResultFile);
            var xDoc = XDocument.Load(testResultFile);

            foreach (var testType in ((IEnumerable) xDoc.XPathEvaluate("//test/@type")).OfType<XAttribute>())
            {
                testType.Value = frameworkName + "+" + testType.Value;
            }

            foreach (var testName in ((IEnumerable) xDoc.XPathEvaluate("//test/@name")).OfType<XAttribute>())
            {
                testName.Value = frameworkName + "+" + testName.Value;
            }

            xDoc.Save(testResultFile);
        }
    }

    string GetFrameworkNameFromFilename(string filename)
    {
        var name = Path.GetFileName(filename);
        name = name.Substring(0, name.Length - ".xml".Length);
        var startIndex = name.LastIndexOf('-');
        name = name.Substring(startIndex + 1);
        return name;
    }
}

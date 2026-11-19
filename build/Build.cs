using static Fallout.Common.IO.Globbing;
using Nuke.CoberturaConverter;
using Fallout.Common.Git;
using Fallout.Common.Tools.DotCover;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Tools.ReportGenerator;
using Fallout.Common;
using Fallout.Common.Tooling;
using Fallout.Common.Utilities;
using Fallout.Common.Utilities.Collections;
using Fallout.GitHub;
using Fallout.WebDocu;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using static Nuke.CoberturaConverter.CoberturaConverterTasks;
using static Nuke.CodeGeneration.CodeGenerator;
using static Fallout.Common.ChangeLog.ChangelogTasks;
using static Fallout.Common.Tools.DotCover.DotCoverTasks;
using static Fallout.Common.Tools.DotNet.DotNetTasks;
using static Fallout.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Fallout.Common.EnvironmentInfo;
using static Fallout.Common.IO.PathConstruction;
using static Fallout.Common.Tooling.ProcessTasks;
using static Fallout.GitHub.ChangeLogExtensions;
using static Fallout.GitHub.GitHubTasks;
using static Fallout.WebDocu.WebDocuTasks;
using Fallout.Common.Tools.AzureKeyVault;
using Fallout.Common.Tools.AzureKeyVault;
using Fallout.Common.IO;

class Build : FalloutBuild
{
    // Console application entry. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Compile);

    [AzureKeyVaultConfiguration(
        BaseUrlParameterName = nameof(KeyVaultBaseUrl),
        ClientIdParameterName = nameof(KeyVaultClientId),
        ClientSecretParameterName = nameof(KeyVaultClientSecret))]
    readonly AzureKeyVaultConfiguration KeyVaultSettings;

    [Parameter] readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Parameter] string KeyVaultBaseUrl;
    [Parameter] string KeyVaultClientId;
    [Parameter] string KeyVaultClientSecret;
    [GitVersion(Framework = "net6.0")] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository GitRepository;

    [AzureKeyVaultSecret] string DocuBaseUrl;
    [AzureKeyVaultSecret] string GitHubAuthenticationToken;
    [AzureKeyVaultSecret] string PublicMyGetSource;
    [AzureKeyVaultSecret] string PublicMyGetApiKey;
    [AzureKeyVaultSecret("CoberturaConverter-DocuApiKey")] string DocuApiKey;
    [AzureKeyVaultSecret] string NuGetApiKey;

    string DocFxFile => RootDirectory / "docfx.json";

    string ChangeLogFile => RootDirectory / "CHANGELOG.md";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(directory => directory.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(directory => directory.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
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
                .SetFileVersion(GitVersion.AssemblySemFileVer)
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
            var testProjects = GlobFiles(RootDirectory / "test", "**/*.csproj");
            var testRun = 1;
            foreach (var testProject in testProjects)
            {
                var projectDirectory = Path.GetDirectoryName(testProject);


                DotNetTest(x => x
                .SetNoBuild(true)
                .SetProjectFile(testProject)
                .SetTestAdapterPath(".")
                .SetLoggers($"xunit;LogFilePath={OutputDirectory / $"test_{testRun++}.testresults.xml"}"));
            }
        });

    Target Coverage => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var testProjects = GlobFiles(RootDirectory / "test", "**/*.csproj").ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                var testProject = testProjects[i];
                var projectDirectory = Path.GetDirectoryName(testProject);
                // This is so that the global dotnet is used instead of the build tool's copy
                var dotnetPath = ToolPathResolver.GetPathExecutable("dotnet");
                var snapshotIndex = i;

                string xUnitOutputDirectory = OutputDirectory / $"test_{snapshotIndex:00}.testresults.xml";
                DotCoverCover(c => c
                    .SetTargetExecutable(dotnetPath)
                    .SetTargetWorkingDirectory(projectDirectory)
                    .SetTargetArguments($"test --no-build --test-adapter-path:. \"--logger:xunit;LogFilePath={xUnitOutputDirectory}\"")
                    .SetFilters("+:CoberturaConverter.Core")
                    .SetAttributeFilters("System.CodeDom.Compiler.GeneratedCodeAttribute")
                    .SetOutputFile(OutputDirectory / $"coverage{snapshotIndex:00}.snapshot"));
            }

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
                .SetFramework("netcoreapp3.0")
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
        .Requires(() => Configuration == "Release")
        .Executes(() =>
        {
            var packages = OutputDirectory.GlobFiles("*.nupkg");
            Assert.NotEmpty(packages);
            packages.Where(x => !x.Name.EndsWith("symbols.nupkg"))
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
            var docFxPath = NuGetToolPathResolver.GetPackageExecutable("docfx", "tools/net10.0/any/docfx.dll");
            DotNet($"{docFxPath} metadata {DocFxFile}");
        });

    Target TestMarkdownDocumentation => _ => _
        .Executes(() => MarkdownDocumentationTests.Run(OutputDirectory / "markdown-tests"));

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

            var docFxPath = NuGetToolPathResolver.GetPackageExecutable("docfx", "tools/net10.0/any/docfx.dll");
            DotNet($"{docFxPath} {DocFxFile}");
            MarkdownDocumentation.Create(DocFxFile, OutputDirectory / "docs-markdown");

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
                .SetMarkdownPackagePath(OutputDirectory / "docs-markdown.zip")
                .SetSourceDirectory(OutputDirectory / "docs")
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
            var nuGetPackages = OutputDirectory.GlobFiles("*.nupkg")
                .Select(path => path.ToString()).ToArray();
            Assert.NotEmpty(nuGetPackages);

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
        var testResults = OutputDirectory.GlobFiles("*.testresults*.xml");
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

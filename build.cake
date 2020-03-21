#addin nuget:?package=Cake.Git

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = File("./NBomber.sln");
var project = File("./src/NBomber/NBomber.fsproj");

var packageVersion = XmlPeek(project, "//Version");
var packageName = "NBomber";

var cloneBranchName = "master";
var reposDirPath = Directory($"./.nbomber-plugins/{packageVersion}");

var repoInfos = new[]
{
    CreateRepoInfo("https://github.com/PragmaticFlow/NBomber.Http.git", 
        reposDirPath,
        File("./src/NBomber.Http/NBomber.Http.fsproj")),

    CreateRepoInfo("https://github.com/PragmaticFlow/NBomber.Sinks.InfluxDB.git", 
        reposDirPath,
        File("./src/NBomber.Sinks.InfluxDB/NBomber.Sinks.InfluxDB.fsproj"))

    // CreateRepoInfo("https://github.com/Yaroshvitaliy/NBomber.Sinks.ReportPortal.git", 
    //     reposDirPath,
    //     File("./src/NBomber.Sinks.ReportPortal/NBomber.Sinks.ReportPortal.fsproj"))
};

var buildTaskName = "Build";
var testTaskName = "Test";

var commitMessage = $"Updated package {packageName} to {packageVersion} version";
var committerName = "antyadev";
var committerEmail = "antyadev@gmail.com";

public RepoInfo CreateRepoInfo(string sourceUrl, ConvertableDirectoryPath reposDirPath, params ConvertableFilePath[] relativeProjPaths)
{
    var repoDirName = sourceUrl.Substring(sourceUrl.LastIndexOf("/") + 1);
    var repoDirPath = reposDirPath + Directory(repoDirName);
    var projPaths = relativeProjPaths.Select(x => repoDirPath + x).ToArray();
    var repoInfo = new RepoInfo(sourceUrl, repoDirPath, projPaths);
    return repoInfo;
}

public class RepoInfo
{
    public RepoInfo(string sourceUrl, ConvertableDirectoryPath repoDirPath, params ConvertableFilePath[] projPaths)
    {
        SourceUrl = sourceUrl;
        RepoDirPath = repoDirPath;
        ProjPaths = projPaths;
    }

    public string SourceUrl { get; }
    public ConvertableDirectoryPath RepoDirPath { get; }
    public ConvertableFilePath[] ProjPaths { get; }
}

public int RunCakeTask(string dirPath, string taskName)
{
    var platform = Environment.OSVersion.Platform; 
    var isLinuxOrMac = platform == PlatformID.Unix || platform == PlatformID.MacOSX;
    var filePath = isLinuxOrMac ? $"{dirPath}/build.sh" : $"{dirPath}/build.ps1";
    var exitCode = StartProcess(filePath, new ProcessSettings { Arguments = $"--target {taskName}" });
    return exitCode;
}

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./src/**/obj");
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/examples/**/obj");
    CleanDirectories("./src/examples/**/bin");
    CleanDirectories("./tests/**/obj");
    CleanDirectories("./tests/**/bin");
    CleanDirectories("./artifacts/");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Information("NBomber Version: {0}", packageVersion);

    DotNetCoreBuild(solution, new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore"),
    });

    DotNetCoreBuild(project, new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore")
    });
});

Task("Test")
    .Does(() =>
{
    var projects = GetFiles("./tests/**/*.fsproj");
    foreach(var project in projects)
    {
        Information("Testing project " + project);

        DotNetCoreTest(project.ToString(),
            new DotNetCoreTestSettings()
            {
                Configuration = configuration,
                NoBuild = true,
                ArgumentCustomization = args => args.Append("--no-restore"),
            });
    }
});

Task("Pack")
	.IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        OutputDirectory = "./artifacts/",
        NoBuild = true,
        Configuration = configuration
    };

	DotNetCorePack(project, settings);
});

Task("CloneNBomberPluginsAndUpdatePackage")
    .Does(() =>
{
    var packageVersionXpath = $"/Project/ItemGroup/PackageReference[@Include = '{packageName}']/@Version";

    if (DirectoryExists(reposDirPath))
    {
        DeleteDirectory(reposDirPath, 
            new DeleteDirectorySettings { Recursive = true, Force = true });
    }

    EnsureDirectoryExists(reposDirPath);

    foreach (var repoInfo in repoInfos)
    {
        Information("Cloning {0} branch for repository {1} to directory {2}", 
            cloneBranchName, repoInfo.SourceUrl, repoInfo.RepoDirPath);
        
        GitClone(repoInfo.SourceUrl, repoInfo.RepoDirPath, 
            new GitCloneSettings { BranchName = cloneBranchName });

        foreach (var projPath in repoInfo.ProjPaths)
        {
            Information("Updating NuGet package to {0} version for project {1}", packageVersion, projPath);
            XmlPoke(projPath, packageVersionXpath, packageVersion);            
        }
    }

    RunTarget("BuildAndTestNBomberPlugins");
})
.ReportError(ex =>
{  
    Error("An error has occurred: {0}", ex.ToString());
});

Task("BuildAndTestNBomberPlugins")
    .Does(() =>
{
    foreach (var repoInfo in repoInfos)
    {
        Information("---------");
        Information("Building repository {0}", repoInfo.RepoDirPath);
        RunCakeTask(repoInfo.RepoDirPath, buildTaskName);
        
        Information("---------");
        Information("Testing repository {0}", repoInfo.RepoDirPath);
        RunCakeTask(repoInfo.RepoDirPath, testTaskName);
    }
})
.ReportError(ex =>
{  
    Error("An error has occurred: {0}", ex.ToString());
});

Task("CommitNBomberPlugins")
    .Does(() =>
{
    foreach (var repoInfo in repoInfos)
    {
        Information("Commiting all changes of repository {0}", repoInfo.RepoDirPath);
        GitAddAll(repoInfo.RepoDirPath);
        GitCommit(repoInfo.RepoDirPath, committerName, committerEmail, commitMessage);
        GitTag(repoInfo.RepoDirPath, packageVersion);
    }
})
.ReportError(ex =>
{  
    Error("An error has occurred: {0}", ex.ToString());
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);

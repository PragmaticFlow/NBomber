#addin nuget:?package=Cake.Git&version=0.21.0

using LibGit2Sharp;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var committerName = Argument("username", "cake build script");
var committerEmail = Argument("email", "pragmaticflow.org@gmail.com");
var committerPassword = Argument("password", "");

var solution = File("./NBomber.sln");
var project = File("./src/NBomber/NBomber.fsproj");
var nbomberVersion = XmlPeek(project, "//Version");

var pluginsDir = Directory($"./.nbomber-plugins/{nbomberVersion}");
var plugins = new[]
{
    new PluginInfo 
    { 
        GitUrl = "https://github.com/PragmaticFlow/NBomber.Http.git",        
        DirPath = pluginsDir + Directory("NBomber.Http"),
        ProjPath = File("./src/NBomber.Http/NBomber.Http.fsproj"),
        SolutionPath = File("./NBomber.Http.sln")
    },
    new PluginInfo 
    { 
        GitUrl = "https://github.com/PragmaticFlow/NBomber.Sinks.InfluxDB.git",        
        DirPath = pluginsDir + Directory("NBomber.Sinks.InfluxDB"),
        ProjPath = File("./src/NBomber.Sinks.InfluxDB/NBomber.Sinks.InfluxDB.fsproj"),
        SolutionPath = File("./NBomber.Sinks.InfluxDB.sln")
    }
};

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
    DotNetCoreRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Information("NBomber Version: {0}", nbomberVersion);

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

Task("BuildPlugins")
    .Does(() =>
{
    if (DirectoryExists(pluginsDir))    
        DeleteDirectory(pluginsDir, new DeleteDirectorySettings { Recursive = true, Force = true });    

    EnsureDirectoryExists(pluginsDir);

    foreach (var plugin in plugins)
    {
        var branchName = "dev";
        var projPath = plugin.GetAbsoluteProjPath();
        var slnPath = plugin.GetAbsoluteSolutionPath();

        Information("Cloning {0} branch for repository {1} to directory {2}", 
            branchName, plugin.GitUrl, pluginsDir);
        
        GitClone(plugin.GitUrl, plugin.DirPath, new GitCloneSettings { BranchName = branchName });        

        // update plugin project version
        Information("Updating plugin project version on '{0}'", nbomberVersion);
        var pluginVersionXPath = "/Project/PropertyGroup/Version";        
        XmlPoke(projPath, pluginVersionXPath, nbomberVersion);        
        
        // update NBomber reference version
        Information("Updating NBomber reference package on '{0}'", nbomberVersion);        
        var nbomberReferenceVersionXPath = "/Project/ItemGroup/PackageReference[@Include = 'NBomber']/@Version";                
        XmlPoke(projPath, nbomberReferenceVersionXPath, nbomberVersion);                

        // update appveyor.yml
        Information("Updating appveyor.yml");        
        var appveyorPath = System.IO.Path.Combine(plugin.DirPath, "appveyor.yml");
        var appveyorContent = System.IO.File.ReadAllLines(appveyorPath);
        appveyorContent[0] = String.Format("version: {0}-{{build}}", nbomberVersion);
        System.IO.File.WriteAllLines(appveyorPath, appveyorContent);

        // build and test plugin
        Information("Build plugin solution");        
        DotNetCoreBuild(slnPath, new DotNetCoreBuildSettings { Configuration = configuration });

        // commit all changes
        Information("Commiting all changes");
        GitAddAll(plugin.DirPath);

        try 
        {
            // commit can fail in case of 0 changes
            GitCommit(plugin.DirPath, committerName, committerEmail, String.Format("version: {0}", nbomberVersion));
        }
        catch (Exception ex)
        {
            Warning(ex.ToString());
        }
    }    
});

Task("PublishPlugins")
    .Does(() =>
{
    foreach (var plugin in plugins)
    {        
        Information("Publish plugin: '{0}'", plugin.DirPath);        
        GitPush(plugin.DirPath, committerName, committerPassword);
    }
});

Task("MergePluginsToMaster")
    .Does(() =>
{
    if (DirectoryExists(pluginsDir))    
        DeleteDirectory(pluginsDir, new DeleteDirectorySettings { Recursive = true, Force = true });    

    EnsureDirectoryExists(pluginsDir);
    
    foreach (var plugin in plugins)
    {        
        Information("clone master branch for plugin: '{0}'", plugin.GitUrl);
        GitClone(plugin.GitUrl, plugin.DirPath, new GitCloneSettings { BranchName = "master" });        

        Information("merge dev into master");
        using (var repo = new Repository(plugin.DirPath))
        {
            var comitterInfo = new Signature(committerName, committerEmail, System.DateTime.UtcNow);
            var mergeResult = repo.Merge(repo.Branches["remotes/origin/dev"], comitterInfo);
        }

        Information("add tag");
        GitTag(plugin.DirPath, $"version-{nbomberVersion}");
    }
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);

public class PluginInfo
{
    public string GitUrl { get; set; }
    public ConvertableDirectoryPath DirPath { get; set; }
    public ConvertableFilePath ProjPath { get; set; }
    public ConvertableFilePath SolutionPath { get; set; }    
    public ConvertableFilePath GetAbsoluteProjPath () => DirPath + ProjPath;
    public ConvertableFilePath GetAbsoluteSolutionPath () => DirPath + SolutionPath;
}

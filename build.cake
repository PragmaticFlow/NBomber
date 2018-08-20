var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.1.0";

var solution = File("./NBomber.sln");
var project = File("./src/NBomber/NBomber.fsproj");

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
    Information("NBomber Version: {0}", version);

    DotNetCoreBuild(solution, new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore"),
    });

    DotNetCoreBuild(project, new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore")
                                            .Append($"/property:Version={version}"),
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
	NuGetPack("./NBomber.nuspec", new NuGetPackSettings
	{
        Version = version,
		OutputDirectory = "./artifacts/"		
	});
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
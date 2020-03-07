var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

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
	var version = XmlPeek(project, "//Version");
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

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);

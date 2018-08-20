var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var nbomberVersion = EnvironmentVariable("nbomberVersion") ?? "0.1.0";
var nugetKey = "vV8UszS9IRwoiZREfiaIByVCYCScXK+PxMOd05VM0nylfMA8DXIrcePPHkQ6bICM";

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./src/**/obj");
    CleanDirectories("./src/**/bin");    
    CleanDirectories("./src/examples/**/obj");
    CleanDirectories("./src/examples/**/bin");
    CleanDirectories("./tests/**/obj");
    CleanDirectories("./tests/**/bin");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./NBomber.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    Information("NBomber Version: {0}", nbomberVersion);

    DotNetCoreBuild("./NBomber.sln", new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore"),
    });

    DotNetCoreBuild("./src/NBomber/NBomber.fsproj", new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore")
                                            .Append($"/property:Version={nbomberVersion}"),
    });
});

Task("Pack")   
    .IsDependentOn("Build")
    .Does(() =>
{
	NuGetPack("./NBomber.nuspec", new NuGetPackSettings
	{
        Version = nbomberVersion,
		OutputDirectory = "./artifacts/"		
	});    
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
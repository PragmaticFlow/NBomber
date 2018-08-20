var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = EnvironmentVariable("version") ?? "0.1.0";
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
    Information("NBomber Version: {0}", version);

    DotNetCoreBuild("./NBomber.sln", new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore"),
    });

    DotNetCoreBuild("./src/NBomber/NBomber.fsproj", new DotNetCoreBuildSettings()
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--no-restore")
                                            .Append($"/property:Version={version}"),
    });
});

Task("Pack")    
    .Does(() =>
{
    Information("NBomber Version: {0}", version);    

	NuGetPack("./NBomber.nuspec", new NuGetPackSettings
	{
        Version = version,
		OutputDirectory = "./artifacts/"		
	});    
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);
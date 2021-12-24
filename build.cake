//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var rid = Argument("rid", "win-x64");
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDir = Directory("./src/RepeatableTimer/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild(
        "./RepeatableTimer.sln",
        new DotNetBuildSettings
        {
            Configuration = configuration
        });
});

Task("Publish")
    .Does(() =>
{
    CleanDirectory("executable");

    DotNetPublish(
        "./src/RepeatableTimer/RepeatableTimer.csproj",
        new DotNetPublishSettings
        {
            Configuration = configuration,
            OutputDirectory = "executable",
            Runtime = rid,
            PublishSingleFile = true,
            SelfContained = true,
            IncludeNativeLibrariesForSelfExtract = true,
            EnableCompressionInSingleFile = true,
        });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

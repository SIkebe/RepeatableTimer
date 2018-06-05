#tool "nuget:?package=ilmerge&version=2.14.1208"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

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

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./RepeatableTimer.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("./RepeatableTimer.sln", settings =>
        settings.SetConfiguration(configuration));
});

Task("ILMerge")
    .IsDependentOn("Build")
    .Does(() =>
{
    var assemblyPaths = GetFiles("./src/RepeatableTimer/bin/Release/*.dll");
    if(!System.IO.Directory.Exists("./artifact"))
    {
        System.IO.Directory.CreateDirectory("./artifact");
    }

    ILMerge(
        "./artifact/RepeatableTimer.exe",
        "./src/RepeatableTimer/bin/Release/RepeatableTimer.exe",
        assemblyPaths,
        new ILMergeSettings { Internalize = true });
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
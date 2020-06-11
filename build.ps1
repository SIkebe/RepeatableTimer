dotnet tool restore
dotnet format
dotnet cake ./build.cake $args
exit $LASTEXITCODE
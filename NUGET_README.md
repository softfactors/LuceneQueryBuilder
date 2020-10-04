Publishing a new package version:

To build new package:

1. In Visual Studio: Build -> Configuration Manager, set LuceneQueryBuilder project to "Release"
2. Increase the AssemblyInfo version in both projects
2. Increase the version in the `.nuspec` file
3. Build solution
4. From within the LuceneQueryBuilder project directory, run `nuget pack -IncludeReferencedProjects -properties Configuration=Release`

Publishing the updated package with

nuget push LuceneQueryBuilder.PACKAGE_VERSION.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY

Then, update the .nuspec file with the commit info of the current package version in the `repository` tag.
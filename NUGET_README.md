Publishing a new package version
================================

1. Update CHANGELOG file with summary of changes
2. Increase the AssemblyInfo version in both projects
3. In Visual Studio: Build -> Configuration Manager, set LuceneQueryBuilder project to "Release"
4. Increase the version in the `.nuspec` file
5. Build solution
6. Commit in git ("bump version to x.y") and tag commit with version number ("vx.y")
7. Copy git hash into the `.nuspec` `repository` tag's `commit` attribute
8. Commit changes in git and push (including the tag)
9. From within the LuceneQueryBuilder project directory (containing the `.nuspec` file), run `nuget pack -IncludeReferencedProjects -properties Configuration=Release`
10. Publish the updated package with

```
nuget push LuceneQueryBuilder.PACKAGE_VERSION.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey YOUR_API_KEY
```
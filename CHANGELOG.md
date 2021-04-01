2.0
===

* main LuceneQueryBuilder library was updated to use .Net standard 2.0
* Test library was updated to use .Net 5
* updated dependent NuGet package versions

1.4
===

Fixes
-----

Blank values are accepted as constraint values: these previously threw `ArgumentException`

1.3
===

Enhancements
------------

Added support for empty expressions

Added overload to `MatchAll` and `MatchAny` collection matchers to accept `IEnumerable<Expression>` as parameter

Tokenizers now filter out empty strings (i.e. they aren't returned)
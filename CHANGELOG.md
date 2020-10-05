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
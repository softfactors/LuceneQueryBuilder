1.3
===

Enhancements
------------

Added support for empty expressions

Added overload to `MatchAll` and `MatchAny` collection matchers to accept `IEnumerable<Expression>` as parameter

Tokenizers now filter out empty strings (i.e. they aren't returned)
LuceneQueryBuilder
==================

Building Queries
----------------

The `Matcher` static class lets you create query `Expression`s with a fluent API, which can then be serialized to string:

```cs
using static LuceneQueryBuilder.Query.Matcher;

var query = Match("name", "Joh*")
	.And("city", "Zürich")
	.Not("name", "Johannes")
	.Build();
```

will match "John" and "Johanna" living in "Zürich", but not "Johannes". The resulting query will be

```cs
"((name:Joh* AND city:Zürich) NOT name:Johannes)"
```

Since every call to `Match` will yield a new expression, managing grouping and priority is straightforward:

```cs
using static LuceneQueryBuilder.Query.Matcher;

var query = Match(
	Match("name", "John*")
	.And(Match("city", "Zürich").Or("city", "Seattle"))
)
.Or(
	Match("name", "Joha*")
	.And(Match("city", "Paris").Or("city", "London"))
)
.Build();
```

will group the sub-queries as expected:

```cs
"((name:John* AND (city:Zürich OR city:Seattle)) OR (name:Joha* AND (city:Paris OR city:London)))"
```

Tokenizing Terms
----------------

The `Tokenize` function provides a convenient mechanism to tokenize search terms according to Lucene analyzers (although only the Keyword, Simple, and Whitelist analyzers are implemented at this time),
or a provided string representing a regular expression:

```cs
using LuceneQueryBuilder.Tokenization;
using static LuceneQueryBuilder.Tokenization.Tokenizer;

var term = "foo/bar0flim flam";

Tokenize(term, Analyzer.Keyword); // ["foo/bar0flim flam"]
Tokenize(term, Analyzer.Simple); // ["foo", "bar", "flim", "flam"]
Tokenize(term, Analyzer.Whitespace); // ["foo/bar0flim", "flam"]
Tokenize(term, @"[\d/]"); // ["foo", "bar", "flim flam"]
```

Naturally, the list of token can then be manipulated to create a query, for example:

```cs
using LuceneQueryBuilder.Tokenization;
using static LuceneQueryBuilder.Tokenization.Tokenizer;

Tokenize("foo/bar0flim flam", Analyzer.Simple)
	.Select(x => Match("name", $"{x}*"))
	.Aggregate((acc, x) => acc.And(x))
	.Build()
```

will yield

```cs
"(((name:foo* AND name:bar*) AND name:flim*) AND name:flam*)"
```

Util
----

The `Util` class provides utilities such as search term escaping (per https://docs.couchdb.org/en/master/ddocs/search.html#query-syntax)

```cs
using static LuceneQueryBuilder.Util;

EscapeTerm("foo-bar!") // "foo\-bar\!"
```
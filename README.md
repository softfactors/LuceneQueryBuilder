Quickstart
==========

```cs
using static LuceneQueryBuilder.Query.Matcher;
using static LuceneQueryBuilder.Tokenization.Tokenizer;

var query = Match("author", "Cha*")
	.And(MatchAll("title", Tokenize("tal two cit").Select(t => $"{t}*")))
	.Not("type", "abridged")
	.Build();
```

will yield

```cs
"((author:Cha* AND ((title:tal* AND title:two*) AND title:cit*)) NOT type:abridged)"
```

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

Queries don't Mutate
--------------------

Queries aren't mutated: you must assign their return value.

You shouldn't do this:

```cs
var query = Match("foo", "bar");

// wrong: the return value isn't assigned and therefore gets lost
query.And("fizz", "buzz");

query.Build(); // "foo:bar"
```

but this

```cs
var query = Match("foo", "bar");

// assign the value to track the updated expression
query = query.And("fizz", "buzz");

query.Build(); // "(foo:bar AND fizz:buzz)"
```

Syntactic Sugar
---------------

```cs
Match("foo", "bar").And("foo", "buzz");
```

can be written as

```cs
MatchAll(
	Match("foo", "bar"),
	Match("foo", "buzz")
);
```

or

```cs
MatchAll(
	new [] {
		Match("foo", "bar"),
		Match("foo", "buzz")
	}
);
```

or simply

```cs
MatchAll("foo", new [] {"bar", "buzz"});
```

The equivalent syntactic sugar for composing the `Or` operation is provided by `MatchAny`.

Empty Expressions
-----------------

Empty expressions are essentially ignored during composition, and building an empty expression will yield `"*:*"` (i.e. a query returning all results):

```cs
using static LuceneQueryBuilder.Query.Matcher;

var nonEmpty = Match("fieldA", "A");
var empty = MatchAll("fieldB", Enumerable.Empty<string>());

nonEmpty.And(empty).Build(); // "fieldA:A"
empty.And(nonEmpty).Build(); // "fieldA:A"

nonEmpty.Or(empty).Build(); // "fieldA:A"
empty.Or(nonEmpty).Build(); // "fieldA:A"

empty.Build(); // "*:*"
empty.And(empty).Build(); // "*:*"
empty.Or(empty).Build(); // "*:*"
```

Queries involving the "NOT" operator behave slightly differently when composing with an empty expression:

```cs
nonEmpty.Not(empty).Build(); // "fieldA:A"
empty.Not(empty).Build(); // "*:*"
empty.Not(nonEmpty).Build(); // "(*:* NOT fieldA:A)"
```

Util
----

The `Util` class provides utilities such as search term escaping (per https://docs.couchdb.org/en/master/ddocs/search.html#query-syntax)

```cs
using static LuceneQueryBuilder.Util;

EscapeTerm("foo-bar!") // "foo\-bar\!"
```

Advanced Usage
==============

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

Traversing Expressions
----------------------

`Expression`s are modeled as a simple `Constraint` (e.g. "foo:bar~") or as having a `Left` sub-`Expression`, an `Operator`, and a `Right` sub-`Expression`.
This means that they can easily be traversed if necessary. Here's an example of traversing an `Expression` in an extension method to ensure the query is scoped
to some field (e.g. the query must be scoped to a tenant field in a multi-tenant application):

```cs
public static class ExpressionExtensions
{
    public static Expression ThrowUnlessScopedTo(this Expression e, string field)
    {
        if (!HasScopeConstraint(e, field))
        {
            throw new Exception($"Expression must be scoped by field '{field}'");
        }

        return e;
    }

    private static bool HasScopeConstraint(Expression e, string field)
    {
        if (e.IsConstraint)
        {
            return e.GetConstraint().Field == field;
        }

        switch (e.GetOperator())
        {
            case Operator.Or:
                return HasScopeConstraint(e.GetLeft(), field) && HasScopeConstraint(e.GetRight(), field);
            case Operator.And:
                return HasScopeConstraint(e.GetLeft(), field) || HasScopeConstraint(e.GetRight(), field);
            case Operator.Not:
                return HasScopeConstraint(e.GetLeft(), field);
            default:
                throw new Exception($"Unknown operator: {e.GetOperator()}");
        }
    }
}
```
using System.Linq;
using LuceneQueryBuilder.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static LuceneQueryBuilder.Query.Matcher;
using static LuceneQueryBuilder.Tokenization.Tokenizer;

namespace Test.Query
{
    [TestClass]
    public class MatcherTest
    {
        private static void AssertSerialization(string expected, Expression actual)
        {
            Assert.AreEqual(expected, actual.Build());
        }

        [TestMethod]
        public void MatchCreatesExpression()
        {
            var match = Match("fieldA", "A");
            var expression = new Expression(new Constraint("fieldA", "A"));

            Assert.AreEqual(expression.Build(), match.Build());
        }

        [TestMethod]
        public void TestMatchAll()
        {
            var a = Match("foo", "bar").And("foo", "buzz");

            var b = MatchAll(
                Match("foo", "bar"),
                Match("foo", "buzz")
            );

            var c = MatchAll(
                new[] {
                    Match("foo", "bar"),
                    Match("foo", "buzz")
                }
            );

            var d = MatchAll("foo", new[] { "bar", "buzz" });

            var e = MatchAll(Tokenize("bar buzz").Select(t => Match("foo", t)));

            foreach (var expression in new[]{ a, b, c, d, e })
            {
                AssertSerialization("(foo:bar AND foo:buzz)", expression);
            }
        }

        [TestMethod]
        public void TestMatchAny()
        {
            var a = Match("foo", "bar").Or("foo", "buzz");

            var b = MatchAny(
                Match("foo", "bar"),
                Match("foo", "buzz")
            );

            var c = MatchAny(
                new[] {
                    Match("foo", "bar"),
                    Match("foo", "buzz")
                }
            );

            var d = MatchAny("foo", new[] { "bar", "buzz" });

            var e = MatchAny(Tokenize("bar buzz").Select(t => Match("foo", t)));

            foreach (var expression in new[] { a, b, c, d, e })
            {
                AssertSerialization("(foo:bar OR foo:buzz)", expression);
            }
        }

        [TestMethod]
        public void CollectionMatchersReturnEmptyExpressionOnEmptyCollections()
        {
            var empty = Enumerable.Empty<Expression>().ToList();
            var expectedSerialization = Expression.Empty().Build();

            AssertSerialization(expectedSerialization, MatchAll(empty));
            AssertSerialization(expectedSerialization, MatchAny(empty));
        }
    }
}

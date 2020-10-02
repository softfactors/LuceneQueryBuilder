using System;
using System.Collections.Generic;
using System.Linq;
using LuceneQueryBuilder.Query;
using LuceneQueryBuilder.Tokenization;
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
        public void CanSerializeConstraint()
        {
            AssertSerialization("foo:bar", Match("foo", "bar"));
        }

        [TestMethod]
        public void CanSerializeAnd()
        {
            const string expected = "(foo:bar AND foo:baz)";

            AssertSerialization(expected, Match("foo", "bar").And("foo", "baz"));
            AssertSerialization(expected, Match("foo", "bar").And(Match("foo", "baz")));
        }

        [TestMethod]
        public void CanSerializeOr()
        {
            const string expected = "(foo:bar OR foo:baz)";

            AssertSerialization(expected, Match("foo", "bar").Or("foo", "baz"));
            AssertSerialization(expected, Match("foo", "bar").Or(Match("foo", "baz")));
        }

        [TestMethod]
        public void CanSerializeNot()
        {
            const string expected = "(foo:bar NOT foo:baz)";

            AssertSerialization(expected, Match("foo", "bar").Not("foo", "baz"));
            AssertSerialization(expected, Match("foo", "bar").Not(Match("foo", "baz")));
        }

        [TestMethod]
        public void TestGrouping()
        {
            var a = Match("fieldA", "A");
            var b = Match("fieldB", "B");
            var c = Match("fieldC", "C");

            AssertSerialization("((fieldA:A AND fieldB:B) OR fieldC:C)", a.And(b).Or(c));
            AssertSerialization("(fieldA:A AND (fieldB:B OR fieldC:C))", a.And(b.Or(c)));
        }

        [TestMethod]
        public void CanAggregate()
        {
            var a = Match("fieldA", "A");
            var b = Match("fieldB", "B");
            var c = Match("fieldC", "C");

            var expected = a.And(b).And(c);
            var actual = new[] { a, b, c }.Aggregate((acc, x) => acc.And(x));

            Assert.AreEqual(expected.Build(), actual.Build());
        }

        [TestMethod]
        public void CanTokenize()
        {
            const string term = "foo bar baz8blah";

            Expression AndAll(IEnumerable<string> tokens) => tokens.Select(p => Match("fieldA", p)).Aggregate((acc, x) => acc.And(x));

            // ReSharper disable once RedundantArgumentDefaultValue
            AssertSerialization("((fieldA:foo AND fieldA:bar) AND fieldA:baz8blah)", AndAll(Tokenize(term, Analyzer.Whitespace)));
            AssertSerialization("(((fieldA:foo AND fieldA:bar) AND fieldA:baz) AND fieldA:blah)", AndAll(Tokenize(term, Analyzer.Simple)));
            AssertSerialization($"fieldA:{term}", AndAll(Tokenize(term, Analyzer.Keyword)));
        }

        [TestMethod]
        public void CanTraverseExpression()
        {
            var a = Match("fieldA", "A");
            var b = Match("fieldB", "B");
            var c = Match("fieldC", "C");

            Assert.IsTrue(HasScopeConstraint(a.And(b), "fieldA"));
            Assert.IsFalse(HasScopeConstraint(a.Or(b), "fieldA"));

            Assert.IsTrue(HasScopeConstraint(Match(a.And(b)).Or(a.And(c)), "fieldA"));
            Assert.IsTrue(HasScopeConstraint(Match(a.Or(b)).And(a.And(c)), "fieldA"));
            Assert.IsTrue(HasScopeConstraint(Match(b.Or(a)).And(c.And(a)), "fieldA"));
            Assert.IsFalse(HasScopeConstraint(Match(a.Or(b)).And(a.Or(c)), "fieldA"));
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
                    // ReSharper disable once TailRecursiveCall
                    return HasScopeConstraint(e.GetLeft(), field);
                default:
                    throw new Exception($"Unknown operator: {e.GetOperator()}");
            }
        }
    }
}

using System.Linq;
using LuceneQueryBuilder.Tokenization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static LuceneQueryBuilder.Tokenization.Tokenizer;

namespace Test.Tokenization
{
    [TestClass]
    public class TokenizerTest
    {
        private const string Term = "foo/bar0flim flam";

        [TestMethod]
        public void KeywordTokenizer()
        {
            CollectionAssert.AreEqual(new[]{ Term }, Tokenize(Term, Analyzer.Keyword).ToList());
        }

        [TestMethod]
        public void SimpleTokenizer()
        {
            CollectionAssert.AreEqual(new[]{ "foo", "bar", "flim", "flam" }, Tokenize(Term, Analyzer.Simple).ToList());
        }

        [TestMethod]
        public void WhitespaceTokenizer()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            CollectionAssert.AreEqual(new[] { "foo/bar0flim", "flam" }, Tokenize(Term, Analyzer.Whitespace).ToList());
        }

        [TestMethod]
        public void RegexTokenizer()
        {
            CollectionAssert.AreEqual(new[] { "foo", "bar", "flim flam" }, Tokenize(Term, @"[\d/]").ToList());
        }

        [TestMethod]
        public void TokenizerRejectsEmptyStrings()
        {
            CollectionAssert.AreEqual(new string[] { }, Tokenize("", Analyzer.Keyword).ToList());
            CollectionAssert.AreEqual(new string[] {  }, Tokenize(" 7/3 ", Analyzer.Simple).ToList());
            // ReSharper disable once RedundantArgumentDefaultValue
            CollectionAssert.AreEqual(new string[] { }, Tokenize("  ", Analyzer.Whitespace).ToList());
            CollectionAssert.AreEqual(new string[] { }, Tokenize("@@", "[@]").ToList());
            CollectionAssert.AreEqual(new [] { " ", " " }, Tokenize(" @@ ", "[@]").ToList());
        }
    }
}

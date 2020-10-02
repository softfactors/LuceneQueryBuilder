using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LuceneQueryBuilder.Tokenization
{
    public static class Tokenizer
    {

        /// <summary>Tokenizes <c>term</c> according to the provided Lucene index analyzer.
        ///
        /// Note that not all analyzers are available.
        ///
        /// For more information on analyzers, see https://docs.couchdb.org/en/master/ddocs/search.html#analyzers
        ///
        /// <example>For example:
        /// <code>
        ///    Tokenize("foo/bar0flim flam", Analyzer.Simple)
        /// </code>
        /// returns `new[]{ "foo", "bar", "flim", "flam" }`.
        /// </example>
        /// </summary>
        /// <param name="term">String to escape</param>
        /// <param name="analyzer">Tokenization.Analyzer indicating which tokenization rules to apply</param>
        /// <returns>an enumerable of string tokens composing the <c>term</c></returns>
        public static IEnumerable<string> Tokenize(string term, Analyzer analyzer = Analyzer.Whitespace)
        {
            switch (analyzer)
            {
                case Analyzer.Keyword:
                    return new[] { term };
                case Analyzer.Whitespace:
                    return Tokenize(term, @"\s+");
                case Analyzer.Simple:
                    return Tokenize(term, @"[^\p{L}]");
                default:
                    throw new Exception($"The {analyzer} analyzer is not supported.");
            }
        }

        /// <summary>Tokenizes <c>term</c> according to the <c>boundaryRegex</c> regular expression.
        ///
        /// <example>For example:
        /// <code>
        ///    Tokenize("foo/bar0flim flam", @"[\d/]")
        /// </code>
        /// returns `new[] { "foo", "bar", "flim flam" }`.
        /// </example>
        /// </summary>
        /// <param name="term">String to escape</param>
        /// <param name="boundaryRegex">String of regular expression to use when tokenizing the <c>term</c></param>
        /// <returns>an enumerable of string tokens composing the <c>term</c></returns>
        public static IEnumerable<string> Tokenize(string term, string boundaryRegex) => Regex.Split(term, boundaryRegex);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneQueryBuilder
{
    public static class Util
    {
        // https://docs.couchdb.org/en/master/ddocs/search.html#query-syntax
        public static readonly IReadOnlyCollection<string> CharsToEscape =
            new List<string> { "\\", "+", "-", "&&", "||", "!", "(", ")", "{", "}", "[", "]", "^", "\"", "~", "*", "?", ":", "/" };

        /// <summary>Escapes <c>term</c> according to Lucene query rules.
        ///
        /// See <see>https://docs.couchdb.org/en/master/ddocs/search.html#query-syntax</see>
        /// <example>For example:
        /// <code>
        ///    EscapeTerm("foo-bar!")
        /// </code>
        /// returns "foo\-bar\!".
        /// </example>
        /// </summary>
        /// <param name="term">String to escape</param>
        /// <returns>an escaped string version of the <c>term</c></returns>
        public static string EscapeTerm(string term) => CharsToEscape.Aggregate(new StringBuilder(term), EscapeSubString).ToString();

        private static StringBuilder EscapeSubString(StringBuilder sb, string toEscape) => sb.Replace(toEscape, $"\\{toEscape}");
    }
}

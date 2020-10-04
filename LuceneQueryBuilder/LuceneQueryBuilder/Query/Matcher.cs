using System;
using System.Collections.Generic;
using System.Linq;

namespace LuceneQueryBuilder.Query
{
    public static class Matcher
    {
        /// <summary>
        /// <returns>Returns an expression containing the <c>value</c> constraint on field <c>field</c>.</returns>
        /// </summary>
        public static Expression Match(string field, string value) => new Expression(new Constraint(field, value));

        /// <summary>
        /// <returns>Returns the unmodified expression.</returns>
        /// </summary>
        public static Expression Match(Expression e) => e;

        public static Expression MatchAll(string field, IEnumerable<string> values) =>
            MatchAll(ToArray(field, values));

        public static Expression MatchAll(IEnumerable<Expression> expressions) =>
            MatchAll(expressions.ToArray());

        public static Expression MatchAll(params Expression[] expressions) =>
            Aggregate(expressions, (acc, e) => acc.And(e));

        public static Expression MatchAny(string field, IEnumerable<string> values) =>
            MatchAny(ToArray(field, values));

        public static Expression MatchAny(IEnumerable<Expression> expressions) =>
            MatchAny(expressions.ToArray());

        public static Expression MatchAny(params Expression[] expressions) =>
            Aggregate(expressions, (acc, e) => acc.Or(e));

        private static Expression[] ToArray(string field, IEnumerable<string> values) =>
            values.Select(v => Match(field, v)).ToArray();

        private static Expression Aggregate(IEnumerable<Expression> expressions, Func<Expression, Expression, Expression> aggregator) =>
            expressions.Aggregate(aggregator);
    }
}

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

        /// <summary>
        /// <returns>Returns an expression matching ALL <c>values</c> (i.e. composed with AND) for field <c>field</c>.</returns>
        /// </summary>
        public static Expression MatchAll(string field, IEnumerable<string> values) =>
            MatchAll(ToArray(field, values));

        /// <summary>
        /// <returns>Returns an expression matching ALL <c>expressions</c> (i.e. composed with AND).</returns>
        /// </summary>
        public static Expression MatchAll(IEnumerable<Expression> expressions) =>
            MatchAll(expressions.ToArray());

        /// <summary>
        /// <returns>Returns an expression matching ALL <c>expressions</c> (i.e. composed with AND).</returns>
        /// </summary>
        public static Expression MatchAll(params Expression[] expressions) =>
            Aggregate(expressions, (acc, e) => acc.And(e));

        /// <summary>
        /// <returns>Returns an expression matching ANY <c>value</c> (i.e. composed with OR) for field <c>field</c>.</returns>
        /// </summary>
        public static Expression MatchAny(string field, IEnumerable<string> values) =>
            MatchAny(ToArray(field, values));

        /// <summary>
        /// <returns>Returns an expression matching ANY <c>expression</c> (i.e. composed with OR).</returns>
        /// </summary>
        public static Expression MatchAny(IEnumerable<Expression> expressions) =>
            MatchAny(expressions.ToArray());

        /// <summary>
        /// <returns>Returns an expression matching ANY <c>expression</c> (i.e. composed with OR).</returns>
        /// </summary>
        public static Expression MatchAny(params Expression[] expressions) =>
            Aggregate(expressions, (acc, e) => acc.Or(e));

        private static Expression[] ToArray(string field, IEnumerable<string> values) =>
            values.Select(v => Match(field, v)).ToArray();

        private static Expression Aggregate(IEnumerable<Expression> expressions, Func<Expression, Expression, Expression> aggregator)
        {
            var expressionsList = expressions.ToList();
            return expressionsList.Any() ? expressionsList.Aggregate(aggregator) : Expression.Empty();
        }
    }
}

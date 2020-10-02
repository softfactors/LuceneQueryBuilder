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
    }
}

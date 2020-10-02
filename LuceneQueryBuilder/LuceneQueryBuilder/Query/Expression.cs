using System;
using System.Text;

namespace LuceneQueryBuilder.Query
{
    public class Expression : Buildable
    {
        public Expression(Operator op, Expression left, Expression right = null)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public Expression(Constraint constraint)
        {
            Constraint = constraint;
        }

        /// <summary>
        /// <returns>Returns a string serialization of the query expression, e.g. "(name:foo* OR name:bar~)".</returns>
        /// </summary>
        public string Build() => ToBuilder().ToString();

        protected internal override StringBuilder ToBuilder()
        {
            return IsConstraint
                ? Constraint.ToBuilder()
                : InParens(Left.ToBuilder().Append($" {Operator.ToString().ToUpper()} ").Append(Right.ToBuilder()));
        }

        private static StringBuilder InParens(StringBuilder b) =>
            new StringBuilder("(").Append(b).Append(new StringBuilder(")"));

        /// <summary>
        /// <returns>Returns true if the query expression is a constraint (e.g. "name:foo*").</returns>
        /// </summary>
        public bool IsConstraint => Constraint != null;

        /// <summary>
        /// <returns>Composes the expression with the given constraint using the AND operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression And(string field, string value) => Apply(Operator.And, (field, value));

        /// <summary>
        /// <returns>Composes the expressions with the AND operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression And(Expression e) => Apply(Operator.And, e);

        /// <summary>
        /// <returns>Composes the expression with the given constraint using the OR operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression Or(string field, string value) => Apply(Operator.Or, (field, value));

        /// <summary>
        /// <returns>Composes the expressions with the OR operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression Or(Expression e) => Apply(Operator.Or, e);

        /// <summary>
        /// <returns>Composes the expression with the given constraint using the NOT operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression Not(string field, string value) => Apply(Operator.Not, (field, value));

        /// <summary>
        /// <returns>Composes the expressions with the NOT operator, returning a new expression instance.</returns>
        /// </summary>
        public Expression Not(Expression e) => Apply(Operator.Not, e);

        private Expression Apply(Operator op, (string field, string value) constraint) =>
            Apply(op, new Expression(new Constraint(constraint.field, constraint.value)));

        private Expression Apply(Operator op, Expression e) => new Expression(op, this, e);

        /// <summary>
        /// <returns>Returns the constraint contained by the query expression.</returns>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the query expression isn't a constraint</exception>
        public Constraint GetConstraint()
        {
            if (IsConstraint)
            {
                return Constraint;
            }

            throw new InvalidOperationException("Expression is not a constraint.");
        }

        /// <summary>
        /// <returns>Returns the query expression's operator.</returns>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the query expression is a constraint</exception>
        public Operator GetOperator()
        {
            if (IsConstraint)
            {
                throw new InvalidOperationException("Expression is a constraint: no operator defined.");
            }

            return Operator;
        }

        /// <summary>
        /// <returns>Returns the query expression's left sub-expression.</returns>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the query expression is a constraint</exception>
        public Expression GetLeft()
        {
            if (IsConstraint)
            {
                throw new InvalidOperationException("Expression is a constraint: no left sub-expression defined.");
            }

            return Left;
        }

        /// <summary>
        /// <returns>Returns the query expression's right sub-expression.</returns>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the query expression is a constraint</exception>
        public Expression GetRight()
        {
            if (IsConstraint)
            {
                throw new InvalidOperationException("Expression is a constraint: no right sub-expression defined.");
            }

            return Right;
        }

        private Operator Operator { get; }

        private Expression Left { get; }

        private Expression Right { get; }

        private Constraint Constraint { get; }
    }
}

using System;
using System.Text;

namespace LuceneQueryBuilder.Query
{
    public class Constraint : Buildable
    {
        public Constraint(string field, string value)
        {
            ThrowIfEmpty(field, "field");
            ThrowIfEmpty(value, "value");

            Field = field;
            Value = value;
        }

        private static void ThrowIfEmpty(string s, string paramName)
        {
            if (s == null)
            {
                throw new ArgumentException($"Constraint `{paramName}` cannot be null");
            }

            if (s.Trim() == "")
            {
                throw new ArgumentException($"Constraint `{paramName}` cannot be empty");
            }
        }

        /// <summary>Returns the field on which the constraint is defined.</summary>
        public string Field { get; }

        /// <summary>Returns the constraint applied (e.g. `foo*`).</summary>
        public string Value { get; }

        protected internal override StringBuilder ToBuilder() => new StringBuilder($"{Field}:{Value}");
    }
}

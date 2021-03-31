using System;
using System.Text;

namespace LuceneQueryBuilder.Query
{
    public class Constraint : Buildable
    {
        public Constraint(string field, string value)
        {
            if (field == null) throw new ArgumentException($"`field` cannot be null");
            if (field.Trim() == "") throw new ArgumentException($"`field` cannot be blank");

            Field = field;
            // blank values are allowed, as they could be indexed via the Keyword analyzer
            // and should therefore be searchable
            Value = value ?? throw new ArgumentException($"`value` cannot be null");
        }

        /// <summary>Returns the field on which the constraint is defined.</summary>
        public string Field { get; }

        /// <summary>Returns the constraint applied (e.g. `foo*`).</summary>
        public string Value { get; }

        protected internal override StringBuilder ToBuilder()
        {
            var trimmedValue = Value.Trim();
            return trimmedValue != ""
                ? new StringBuilder($"{Field}:{trimmedValue}")
                : new StringBuilder($"{Field}:\"{Value}\"");
        }
    }
}

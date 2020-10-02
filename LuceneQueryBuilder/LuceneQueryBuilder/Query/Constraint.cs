using System.Text;

namespace LuceneQueryBuilder.Query
{
    public class Constraint : Buildable
    {
        public Constraint(string field, string value)
        {
            Field = field;
            Value = value;
        }

        /// <summary>Returns the field on which the constraint is defined.</summary>
        public string Field { get; }

        /// <summary>Returns the constraint applied (e.g. `foo*`).</summary>
        public string Value { get; }

        protected internal override StringBuilder ToBuilder() => new StringBuilder($"{Field}:{Value}");
    }
}

using System.Text;

namespace LuceneQueryBuilder.Query
{
    public abstract class Buildable
    {
        // ReSharper disable once UnusedMemberInSuper.Global
        protected internal virtual StringBuilder ToBuilder()
        {
            throw new System.NotImplementedException();
        }
    }
}

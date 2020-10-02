namespace LuceneQueryBuilder.Tokenization
{
    // see https://docs.couchdb.org/en/master/ddocs/search.html#analyzers
    // Only implemented analyzers are listed in this enum.
    public enum Analyzer
    {
        Keyword,
        Simple,
        Whitespace
    }
}

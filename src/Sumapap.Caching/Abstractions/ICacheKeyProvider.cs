namespace Sumapap.Caching.Abstractions
{
    public interface ICacheKeyProvider
    {
        string CreateKey<TObject>(TObject @object, params object[] parameters)
            where TObject : class;

        string CreateKey<TObject>(params object[] parameters)
            where TObject : class;
    }
}

namespace Sumapap.Caching.Abstractions
{
    public interface ICacheKeyProvider
    {
        string CreateKey(string @object, params object[] parameters);

        string CreateKey<TObject>(params object[] parameters);
    }
}

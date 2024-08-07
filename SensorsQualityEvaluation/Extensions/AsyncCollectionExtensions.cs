namespace SensorsQualityEvaluation.Extensions;

//or we could just use System.Linq.Async nuget
public static class AsyncCollectionExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items)
    {
        var results = new List<T>();
        await foreach (var item in items)
            results.Add(item);
        return results;
    }

    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.CompletedTask;
        }
    }
}
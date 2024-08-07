namespace SensorsQualityEvaluation.Utils;

public static class MathUtils
{
    public static (double Mean, double StandardDeviation) GetStandardDeviation<T>(
        IReadOnlyCollection<T> records, Func<T, double> selector)
    {
        if (records.Count == 0) 
            return (0.0, 0.0);

        var sum = 0.0;
        var sumOfSquares = 0.0;

        foreach (var record in records)
        {
            var value = selector(record);
            sum += value;
            sumOfSquares += value * value;
        }

        var avg = sum / records.Count;
        var variance = (sumOfSquares - (sum * sum / records.Count)) / (records.Count - 1);
        var stdDev = Math.Sqrt(variance);

        return (avg, stdDev);
    }

    public static bool AllWithinTolerance<T>(IReadOnlyCollection<T> records, Func<T, double> selector, double referenceValue, double tolerance)
    {
        return records.All(record => !(Math.Abs(selector(record) - referenceValue) > tolerance));
    }
}
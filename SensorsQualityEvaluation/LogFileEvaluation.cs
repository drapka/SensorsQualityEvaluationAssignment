namespace SensorsQualityEvaluation;

public interface ILogFileEvaluator
{
    Task Evaluate(string inputPath, string outputPath, CancellationToken token = default);
    Task Evaluate(Stream inputStream, Stream outputStream, CancellationToken token = default);
}
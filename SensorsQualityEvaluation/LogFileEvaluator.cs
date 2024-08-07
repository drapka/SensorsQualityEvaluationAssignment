using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SensorsQualityEvaluation.Services.InputParsing;
using SensorsQualityEvaluation.Services.OutputSerialization;
using SensorsQualityEvaluation.Services.QualityEvaluation;

namespace SensorsQualityEvaluation;

public class LogFileEvaluator(
    ITelemetryInputParserFactory inputParserFactory,
    IQualityEvaluator evaluator,
    IOutputSerializer outputSerializer,
    ILogger<LogFileEvaluator> logger)
    : ILogFileEvaluator
{
    public async Task Evaluate(string inputPath, string outputPath, CancellationToken token = default)
    {
        await using var inStream = File.OpenRead(inputPath);
        await using var outStream = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);

        await Evaluate(inStream, outStream, token);
        logger.LogInformation("Evaluation output has been saved to {fileName}.", outputPath);
    }

    public async Task Evaluate(Stream inputStream, Stream outputStream, CancellationToken token = default)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Starting log file evaluation...");

        using var inputParser = inputParserFactory.Create(inputStream);

        var referenceData = await inputParser.ParseReferenceData(token);
        var telemetryData = inputParser.ParseSensorTelemetry(token);

        var results = evaluator.Evaluate(referenceData, telemetryData, token);

        await outputSerializer.SerializeEvaluationResult(outputStream, results, token);

        logger.LogInformation("Log file evaluation finished in {elapsed}", sw.Elapsed);
    }
}
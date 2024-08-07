using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.QualityEvaluation;

public class DefaultQualityEvaluator(
    ILogger<DefaultQualityEvaluator> logger,
    ISensorEvaluationStrategySelector strategySelector)
    : IQualityEvaluator
{
    public async IAsyncEnumerable<SensorEvaluationResult> Evaluate(
        ReferenceTelemetry referenceTelemetry,
        IAsyncEnumerable<SensorTelemetry> sensorsTelemetry,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await foreach (var sensorTelemetry in sensorsTelemetry.WithCancellation(token))
        {
            logger.LogInformation("Evaluating quality for sensor {sensorName}, type {sensorType}...",
                sensorTelemetry.SensorName, sensorTelemetry.SensorType);

            var strategy = strategySelector.SelectEvaluator(sensorTelemetry.SensorType);
            if (strategy == null)
            {
                logger.LogWarning("Strategy for sensor type {sensorType} is not defined.", sensorTelemetry.SensorType);
                yield return new SensorEvaluationResult(sensorTelemetry.SensorName, EvaluationResult.UnknownStrategy);
                continue;
            }

            if (!referenceTelemetry.Data.TryGetValue(sensorTelemetry.SensorType, out var reference))
            {
                logger.LogWarning("Reference data for sensor type {sensorType} is missing.", sensorTelemetry.SensorType);
                yield return new SensorEvaluationResult(sensorTelemetry.SensorName, EvaluationResult.MissingReference);
                continue;
            }

            var result = await strategy.Evaluate(reference, sensorTelemetry);
            logger.LogInformation("Evaluated quality for sensor {sensorName}, type {sensorType} with result: {result}",
                                  sensorTelemetry.SensorName, sensorTelemetry.SensorType, result);
            yield return new SensorEvaluationResult(sensorTelemetry.SensorName, result);
        }
    }
}
using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.QualityEvaluation;

public interface IQualityEvaluator
{
    IAsyncEnumerable<SensorEvaluationResult> Evaluate(
        ReferenceTelemetry referenceTelemetry,
        IAsyncEnumerable<SensorTelemetry> sensorsTelemetry,
        CancellationToken token = default);
}
using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Utils;

namespace SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

public class MonoxideEvaluationStrategy : ISensorEvaluationStrategy
{
    public string Name => "monoxide";

    public Task<EvaluationResult> Evaluate(double referenceValue, SensorTelemetry sensorTelemetry)
    {
        const double tolerance = 3.0;//TODO: move to options

        if (sensorTelemetry.Telemetry.Count == 0)
            return Task.FromResult(EvaluationResult.MissingData);

        var allWithinTolerance = MathUtils.AllWithinTolerance(sensorTelemetry.Telemetry,
            r => r.Value, referenceValue, tolerance);

        return Task.FromResult(allWithinTolerance ? EvaluationResult.Keep : EvaluationResult.Discard);
    }
}
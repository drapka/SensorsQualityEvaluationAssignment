using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

public interface ISensorEvaluationStrategy
{
    string Name { get; }

    Task<EvaluationResult> Evaluate(double referenceValue, SensorTelemetry sensorTelemetry);
}
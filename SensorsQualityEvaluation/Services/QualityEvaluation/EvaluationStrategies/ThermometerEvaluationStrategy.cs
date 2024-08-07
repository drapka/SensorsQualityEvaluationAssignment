using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Utils;

namespace SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

public class ThermometerEvaluationStrategy : ISensorEvaluationStrategy
{
    public string Name => "thermometer";

    public Task<EvaluationResult> Evaluate(double referenceValue, SensorTelemetry sensorTelemetry)
    {
        //TODO: move to options
        const double meanTolerance = 0.5;
        const double ultraPreciseDeviationMax = 3;
        const double veryPreciseDeviationMax = 5;

        var readings = sensorTelemetry.Telemetry;

        if (readings.Count == 0)
            return Task.FromResult(EvaluationResult.MissingData);

        var (mean, stdDev) = MathUtils.GetStandardDeviation(readings, r => r.Value);

        if (Math.Abs(mean - referenceValue) <= meanTolerance)
        {
            return stdDev switch
            {
                < ultraPreciseDeviationMax => Task.FromResult(EvaluationResult.UltraPrecise),
                < veryPreciseDeviationMax => Task.FromResult(EvaluationResult.VeryPrecise),
                _ => Task.FromResult(EvaluationResult.Precise)
            };
        }

        return Task.FromResult(EvaluationResult.Precise);
    }
}
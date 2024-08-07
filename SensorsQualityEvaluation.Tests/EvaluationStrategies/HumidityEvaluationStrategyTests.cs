using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation.Tests.EvaluationStrategies;

[TestClass]
public class HumidityEvaluationStrategyTests
{
    private readonly HumidityEvaluationStrategy strategy = new();

    [TestMethod]
    public async Task Evaluate_AllReadingsWithinTolerance_ShouldReturnKeep()
    {
        const double referenceValue = 50.0;
        var sensorTelemetry = new SensorTelemetry("humidity", "hum-1", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 49.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 50.5),
            new TelemetryRecord(new DateTime(2024, 8, 7), 49.8)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Keep, result);
    }

    [TestMethod]
    public async Task Evaluate_SomeReadingsOutsideTolerance_ShouldReturnDiscard()
    {
        const double referenceValue = 50.0;
        var sensorTelemetry = new SensorTelemetry("humidity", "hum-2", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 49.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 52.0), // Out of tolerance
            new TelemetryRecord(new DateTime(2024, 8, 7), 49.8)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Discard, result);
    }
}
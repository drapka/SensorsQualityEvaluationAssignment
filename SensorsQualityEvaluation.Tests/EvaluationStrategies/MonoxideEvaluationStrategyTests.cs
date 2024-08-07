using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation.Tests.EvaluationStrategies;

[TestClass]
public class MonoxideEvaluationStrategyTests
{
    private readonly MonoxideEvaluationStrategy strategy = new();

    [TestMethod]
    public async Task Evaluate_AllReadingsWithinTolerance_ShouldReturnKeep()
    {
        const double referenceValue = 50.0;
        var sensorTelemetry = new SensorTelemetry("monoxide", "mon-1", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 48.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 50.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 52.0)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Keep, result);
    }

    [TestMethod]
    public async Task Evaluate_SomeReadingsOutsideTolerance_ShouldReturnDiscard()
    {
        const double referenceValue = 50.0;
        var sensorTelemetry = new SensorTelemetry("monoxide", "mon-2", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 48.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 55.0), // Out of tolerance
            new TelemetryRecord(new DateTime(2024, 8, 7), 52.0)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Discard, result);
    }
}
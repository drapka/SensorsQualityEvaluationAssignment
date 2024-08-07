using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation.Tests.EvaluationStrategies;

[TestClass]
public class ThermometerEvaluationStrategyTests
{
    private readonly ThermometerEvaluationStrategy strategy = new();

    [TestMethod]
    public async Task Evaluate_MeanAndStdDevWithinUltraPreciseLimits_ShouldReturnUltraPrecise()
    {
        const double referenceValue = 25.0;
        var sensorTelemetry = new SensorTelemetry("thermometer", "temp-1", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 24.5),
            new TelemetryRecord(new DateTime(2024, 8, 7), 25.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 25.5)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.UltraPrecise, result);
    }

    [TestMethod]
    public async Task Evaluate_MeanWithinToleranceButStdDevExceedsVeryPreciseLimit_ShouldReturnPrecise()
    {
        const double referenceValue = 25.0;
        var sensorTelemetry = new SensorTelemetry("thermometer", "temp-2", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 23.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 26.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 29.0)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Precise, result);
    }

    [TestMethod]
    public async Task Evaluate_MeanOutsideTolerance_ShouldReturnPrecise()
    {
        const double referenceValue = 25.0;
        var sensorTelemetry = new SensorTelemetry("thermometer", "temp-3", [
            new TelemetryRecord(new DateTime(2024, 8, 7), 20.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 22.0),
            new TelemetryRecord(new DateTime(2024, 8, 7), 23.0)
        ]);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.Precise, result);
    }

    [TestMethod]
    public async Task Evaluate_NoReadings_ShouldReturnMissingData()
    {
        const double referenceValue = 25.0;
        var sensorTelemetry = new SensorTelemetry("thermometer", "temp-4", []);

        var result = await strategy.Evaluate(referenceValue, sensorTelemetry);

        Assert.AreEqual(EvaluationResult.MissingData, result);
    }
}
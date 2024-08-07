using Moq;
using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;
using SensorsQualityEvaluation.Services.QualityEvaluation;
using Microsoft.Extensions.Logging.Abstractions;
using SensorsQualityEvaluation.Extensions;

namespace SensorsQualityEvaluation.Tests;

[TestClass]
public class QualityEvaluatorTests
{
    private Mock<ISensorEvaluationStrategySelector> strategySelectorMock = null!;
    private DefaultQualityEvaluator evaluator = null!;

    [TestInitialize]
    public void Setup()
    {
        strategySelectorMock = new Mock<ISensorEvaluationStrategySelector>();
        evaluator = new DefaultQualityEvaluator(NullLogger<DefaultQualityEvaluator>.Instance, strategySelectorMock.Object);
    }

    [TestMethod]
    public async Task Evaluate_SensorTypeWithDefinedStrategy_ShouldReturnExpectedResult()
    {
        //setup
        var referenceTelemetry = new ReferenceTelemetry(new Dictionary<string, double>
        {
            { "thermometer", 25.0 }
        });

        var sensorTelemetry = new SensorTelemetry("thermometer", "temp-1", [
            new TelemetryRecord(DateTime.UtcNow, 24.5)
        ]);
        var sensorsTelemetry = new List<SensorTelemetry> { sensorTelemetry };

        var strategyMock = new Mock<ISensorEvaluationStrategy>();
        strategyMock.Setup(s => s.Evaluate(It.IsAny<double>(), It.IsAny<SensorTelemetry>()))
            .ReturnsAsync(EvaluationResult.UltraPrecise);

        strategySelectorMock.Setup(s => s.SelectEvaluator("thermometer"))
            .Returns(strategyMock.Object);


        //act
        var results = await evaluator.Evaluate(referenceTelemetry, sensorsTelemetry.ToAsyncEnumerable()).ToListAsync();

        //assert
        Assert.AreEqual(sensorsTelemetry.Count, results.Count);
        Assert.AreEqual(sensorTelemetry.SensorName, results[0].SensorName);
        Assert.AreEqual(EvaluationResult.UltraPrecise, results[0].Result);
    }

    [TestMethod]
    public async Task Evaluate_SensorTypeWithUndefinedStrategy_ShouldReturnUnknownStrategy()
    {
        //setup
        var referenceTelemetry = new ReferenceTelemetry(new Dictionary<string, double>
        {
            { "humidity", 50.0 }
        });

        var sensorTelemetry = new SensorTelemetry("humidity", "hum-1", [
            new TelemetryRecord(DateTime.UtcNow, 49.0)
        ]);
        var sensorsTelemetry = new List<SensorTelemetry> { sensorTelemetry };

        strategySelectorMock.Setup(s => s.SelectEvaluator("humidity"))
            .Returns((ISensorEvaluationStrategy?)null);

        //act
        var results = await evaluator.Evaluate(referenceTelemetry, sensorsTelemetry.ToAsyncEnumerable()).ToListAsync();

        //assert
        Assert.AreEqual(sensorsTelemetry.Count, results.Count);
        Assert.AreEqual(sensorTelemetry.SensorName, results[0].SensorName);
        Assert.AreEqual(EvaluationResult.UnknownStrategy, results[0].Result);
    }

    [TestMethod]
    public async Task Evaluate_MissingReferenceData_ShouldReturnMissingReference()
    {
        //setup
        var referenceTelemetry = new ReferenceTelemetry([]);

        var sensorTelemetry = new SensorTelemetry("monoxide", "mon-1", [
            new TelemetryRecord(DateTime.UtcNow, 5.0)
        ]);
        var sensorsTelemetry = new List<SensorTelemetry> { sensorTelemetry };

        strategySelectorMock.Setup(s => s.SelectEvaluator("monoxide"))
            .Returns(new Mock<ISensorEvaluationStrategy>().Object);

        //act
        var results = await evaluator.Evaluate(referenceTelemetry, sensorsTelemetry.ToAsyncEnumerable()).ToListAsync();

        //assert
        Assert.AreEqual(sensorsTelemetry.Count, results.Count);
        Assert.AreEqual(sensorTelemetry.SensorName, results[0].SensorName);
        Assert.AreEqual(EvaluationResult.MissingReference, results[0].Result);
    }

    [TestMethod]
    public async Task Evaluate_EmptySensorTelemetry_ShouldReturnMissingData()
    {
        //setup
        var referenceTelemetry = new ReferenceTelemetry(new Dictionary<string, double>
        {
            { "humidity", 50.0 }
        });

        var sensorTelemetry = new SensorTelemetry("humidity", "hum-2", []);
        var sensorsTelemetry = new List<SensorTelemetry> { sensorTelemetry };

        strategySelectorMock.Setup(s => s.SelectEvaluator("humidity"))
            .Returns(new Mock<ISensorEvaluationStrategy>().Object);

        //act
        var results = await evaluator.Evaluate(referenceTelemetry, sensorsTelemetry.ToAsyncEnumerable()).ToListAsync();

        //assert
        Assert.AreEqual(results.Count, results.Count);
        Assert.AreEqual(sensorTelemetry.SensorName, results[0].SensorName);
        Assert.AreEqual(EvaluationResult.MissingData, results[0].Result);
    }
}
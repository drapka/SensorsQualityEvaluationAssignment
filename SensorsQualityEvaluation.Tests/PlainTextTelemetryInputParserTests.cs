using System.Text;
using SensorsQualityEvaluation.Extensions;
using SensorsQualityEvaluation.Options;
using SensorsQualityEvaluation.Services.InputParsing;

namespace SensorsQualityEvaluation.Tests;

[TestClass]
public class PlainTextTelemetryInputParserTests
{
    [TestMethod]
    public async Task ParseSensorTelemetry_ValidTelemetry_ShouldReturnCorrectTelemetry()
    {
        const string data = 
            """
            reference 70.0 45.0
            thermometer temp-1
            2007-04-05T22:00 72.4
            2007-04-05T22:01 76.0
            humidity hum-1
            2007-04-05T22:04 45.2
            2007-04-05T22:05 45.3
            """;
        
        var options = new ReferenceDataOptions { Keys = ["thermometer", "humidity"] };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        using var inputParser = new PlainTextTelemetryInputParser(options, stream);
        var referenceData = await inputParser.ParseReferenceData();
        Assert.AreEqual(2, referenceData.Data.Count);
        CollectionAssert.AreEqual(new Dictionary<string, double>
        {
            { "thermometer", 70.0 },
            { "humidity", 45.0 },
        }, referenceData.Data);

        var telemetryData = await inputParser.ParseSensorTelemetry().ToListAsync();
        var thermometerTelemetry = telemetryData[0];
        Assert.AreEqual("thermometer", thermometerTelemetry.SensorType);
        Assert.AreEqual("temp-1", thermometerTelemetry.SensorName);
        Assert.AreEqual(2, thermometerTelemetry.Telemetry.Count);
        Assert.AreEqual(new DateTime(2007, 4, 5, 22, 0, 0), thermometerTelemetry.Telemetry[0].Timestamp);
        Assert.AreEqual(72.4, thermometerTelemetry.Telemetry[0].Value);
        Assert.AreEqual(new DateTime(2007, 4, 5, 22, 1, 0), thermometerTelemetry.Telemetry[1].Timestamp);
        Assert.AreEqual(76.0, thermometerTelemetry.Telemetry[1].Value);

        var humidityTelemetry = telemetryData[1];
        Assert.AreEqual("humidity", humidityTelemetry.SensorType);
        Assert.AreEqual("hum-1", humidityTelemetry.SensorName);
        Assert.AreEqual(2, humidityTelemetry.Telemetry.Count);
        Assert.AreEqual(new DateTime(2007, 4, 5, 22, 4, 0), humidityTelemetry.Telemetry[0].Timestamp);
        Assert.AreEqual(45.2, humidityTelemetry.Telemetry[0].Value);
        Assert.AreEqual(new DateTime(2007, 4, 5, 22, 5, 0), humidityTelemetry.Telemetry[1].Timestamp);
        Assert.AreEqual(45.3, humidityTelemetry.Telemetry[1].Value);
    }
}
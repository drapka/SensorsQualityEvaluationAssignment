using System.Text;
using SensorsQualityEvaluation.Extensions;
using SensorsQualityEvaluation.Models;
using SensorsQualityEvaluation.Services.OutputSerialization;

namespace SensorsQualityEvaluation.Tests;

[TestClass]
public class JsonOutputSerializerTests
{
    [TestMethod]
    public async Task SerializeEvaluationResult_EmptyData_ShouldSerializeEmptyArray()
    {
        var evaluationResults = new List<SensorEvaluationResult>().ToAsyncEnumerable();
        var serializer = new JsonOutputSerializer();
        using var stream = new MemoryStream();

        await serializer.SerializeEvaluationResult(stream, evaluationResults);
        var jsonResult = Encoding.UTF8.GetString(stream.ToArray());

        const string expectedJson = "{}";
        Assert.AreEqual(expectedJson, jsonResult);
    }

    [TestMethod]
    public async Task SerializeEvaluationResult_ValidData_ShouldSerializeCorrectly()
    {
        var evaluationResults = new List<SensorEvaluationResult>
        {
            new("temp-1", EvaluationResult.UltraPrecise),
            new("hum-1", EvaluationResult.Discard)
        }.ToAsyncEnumerable();

        var serializer = new JsonOutputSerializer();
        using var stream = new MemoryStream();

        await serializer.SerializeEvaluationResult(stream, evaluationResults);
        var jsonResult = Encoding.UTF8.GetString(stream.ToArray());

        const string expectedJson = """
                                    {
                                      "temp-1": "ultra precise",
                                      "hum-1": "discard"
                                    }
                                    """;

        Assert.AreEqual(expectedJson, jsonResult);
    }
}
using System.Text;
using System.Text.Json;
using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.OutputSerialization;

public class JsonOutputSerializer : IOutputSerializer
{
    public async Task SerializeEvaluationResult(Stream stream, IAsyncEnumerable<SensorEvaluationResult> evaluationResults, 
        CancellationToken token = default)
    {
        await using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartArray();

        await foreach (var result in evaluationResults.WithCancellation(token))
        {
            token.ThrowIfCancellationRequested();
            WriteSensorEvaluationResult(writer, result);
        }

        writer.WriteEndArray();
        await writer.FlushAsync(token);
    }

    private static void WriteSensorEvaluationResult(Utf8JsonWriter writer, SensorEvaluationResult result)
    {
        writer.WriteStartObject();
        writer.WriteString(result.SensorName, ToSpaceSeparated(result.Result.ToString()));
        writer.WriteEndObject();
    }

    private static string ToSpaceSeparated(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var currentChar = value[i];
            if (i > 0 && char.IsUpper(currentChar))
                result.Append(' ');
            
            result.Append(char.ToLower(currentChar));
        }
        return result.ToString();
    }
}
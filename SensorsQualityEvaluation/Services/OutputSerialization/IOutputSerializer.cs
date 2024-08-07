using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.OutputSerialization;

public interface IOutputSerializer
{
    Task SerializeEvaluationResult(Stream stream, IAsyncEnumerable<SensorEvaluationResult> evaluationResults, 
        CancellationToken token = default);
}
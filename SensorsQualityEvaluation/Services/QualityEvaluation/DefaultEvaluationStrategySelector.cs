using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation.Services.QualityEvaluation;

internal class DefaultEvaluationStrategySelector(
    IEnumerable<ISensorEvaluationStrategy> strategies)
    : ISensorEvaluationStrategySelector
{
    private readonly Dictionary<string, ISensorEvaluationStrategy> strategies = strategies
        .ToDictionary(s => s.Name);

    public ISensorEvaluationStrategy? SelectEvaluator(string sensorType) =>
        strategies.GetValueOrDefault(sensorType);
}
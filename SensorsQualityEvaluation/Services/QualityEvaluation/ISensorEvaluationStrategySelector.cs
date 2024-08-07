using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation.Services.QualityEvaluation;

public interface ISensorEvaluationStrategySelector
{
    ISensorEvaluationStrategy? SelectEvaluator(string sensorType);
}
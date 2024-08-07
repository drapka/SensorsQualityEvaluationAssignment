namespace SensorsQualityEvaluation.Models;

public enum EvaluationResult
{
    //evaluation problems
    MissingData,
    MissingReference,
    UnknownStrategy,

    //actual sensor evaluation results
    UltraPrecise,
    VeryPrecise,
    Precise,
    Keep,
    Discard
}
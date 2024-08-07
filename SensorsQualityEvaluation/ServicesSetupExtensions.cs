using Microsoft.Extensions.DependencyInjection;
using SensorsQualityEvaluation.Options;
using SensorsQualityEvaluation.Services.InputParsing;
using SensorsQualityEvaluation.Services.OutputSerialization;
using SensorsQualityEvaluation.Services.QualityEvaluation;
using SensorsQualityEvaluation.Services.QualityEvaluation.EvaluationStrategies;

namespace SensorsQualityEvaluation;

public static class ServicesSetupExtensions
{
    public static IServiceCollection AddSensorQualityEvaluation(
        this IServiceCollection services, Action<ReferenceDataOptions> referenceDataOptionsConfiguration) => services
        .AddOptions()
        .Configure(referenceDataOptionsConfiguration)
        .AddSingleton<ILogFileEvaluator, LogFileEvaluator>()
        .AddInputParsers()
        .AddOutputParsers()
        .AddEvaluationServices();

    //more could be added in the future - JSON, YAML, APIs, DBs
    private static IServiceCollection AddInputParsers(this IServiceCollection services) => services
        .AddSingleton<ITelemetryInputParserFactory, PlainTextTelemetryInputParserFactory>();

    //again, there could be more - we could select them by key as service provider now supports keyed services...
    private static IServiceCollection AddOutputParsers(this IServiceCollection services) => services
        .AddSingleton<IOutputSerializer, JsonOutputSerializer>();

    private static IServiceCollection AddEvaluationServices(this IServiceCollection services) => services
        .AddSingleton<IQualityEvaluator, DefaultQualityEvaluator>()
        .AddSingleton<ISensorEvaluationStrategySelector, DefaultEvaluationStrategySelector>()
        .AddSingleton<ISensorEvaluationStrategy, ThermometerEvaluationStrategy>()
        .AddSingleton<ISensorEvaluationStrategy, HumidityEvaluationStrategy>()
        .AddSingleton<ISensorEvaluationStrategy, MonoxideEvaluationStrategy>();
}
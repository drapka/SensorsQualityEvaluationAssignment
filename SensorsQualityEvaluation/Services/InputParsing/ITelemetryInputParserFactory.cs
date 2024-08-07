namespace SensorsQualityEvaluation.Services.InputParsing;

public interface ITelemetryInputParserFactory
{
    ITelemetryInputParser Create(Stream stream);
}
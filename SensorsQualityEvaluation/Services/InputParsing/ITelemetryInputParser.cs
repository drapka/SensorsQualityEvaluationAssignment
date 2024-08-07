using SensorsQualityEvaluation.Models;

namespace SensorsQualityEvaluation.Services.InputParsing;

public interface ITelemetryInputParser : IDisposable
{
    Task<ReferenceTelemetry> ParseReferenceData(CancellationToken token = default);

    IAsyncEnumerable<SensorTelemetry> ParseSensorTelemetry(CancellationToken token = default);
}
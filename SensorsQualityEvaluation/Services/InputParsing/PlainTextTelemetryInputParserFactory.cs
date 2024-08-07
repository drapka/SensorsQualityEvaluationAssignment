using Microsoft.Extensions.Options;
using SensorsQualityEvaluation.Options;

namespace SensorsQualityEvaluation.Services.InputParsing;

public class PlainTextTelemetryInputParserFactory(
    IOptions<ReferenceDataOptions> referenceDataOptions)
    : ITelemetryInputParserFactory
{
    public ITelemetryInputParser Create(Stream stream) =>
        new PlainTextTelemetryInputParser(referenceDataOptions.Value, stream);
}
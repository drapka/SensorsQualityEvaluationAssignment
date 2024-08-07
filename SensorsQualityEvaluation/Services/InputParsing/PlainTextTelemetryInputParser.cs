using SensorsQualityEvaluation.Models;
using System.Globalization;
using System.Runtime.CompilerServices;
using SensorsQualityEvaluation.Exceptions;
using SensorsQualityEvaluation.Options;

namespace SensorsQualityEvaluation.Services.InputParsing;

public class PlainTextTelemetryInputParser(ReferenceDataOptions referenceDataOptions, Stream stream) : ITelemetryInputParser
{
    private const char DataSeparator = ' ';
    private readonly StreamReader streamReader = new(stream, leaveOpen: true);
    private ReferenceTelemetry? referenceTelemetry;

    public async Task<ReferenceTelemetry> ParseReferenceData(CancellationToken token = default)
    {
        const string linePrefix = "reference";

        if (referenceTelemetry != null)
            return referenceTelemetry;

        var line = await streamReader.ReadLineAsync(token)
                   ?? throw new InputParsingException("Reference telemetry not found.");

        if (!line.StartsWith(linePrefix))
            throw new InputParsingException($"The first line must start with '{linePrefix}' prefix.");


        var parts = line.Split(DataSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length - 1 != referenceDataOptions.Keys.Count)
            throw new InputParsingException($"Invalid reference telemetry data count. Expected {referenceDataOptions.Keys.Count}, got {parts.Length - 1}");

        var referenceData = parts
            .Skip(1)
            .Select(ParseReferenceValue)
            .ToDictionary();

        referenceTelemetry = new ReferenceTelemetry(referenceData);
        return referenceTelemetry;
    }

    public async IAsyncEnumerable<SensorTelemetry> ParseSensorTelemetry([EnumeratorCancellation] CancellationToken token = default)
    {
        if (referenceTelemetry == null)
            await ParseReferenceData(token);

        SensorTelemetry? currentSensorTelemetry = null;

        while (await streamReader.ReadLineAsync(token) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(DataSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new InputParsingException($"Unknown telemetry line format: {line}");

            if (IsSensorData(parts, out var ts, out var value))
            {
                if (currentSensorTelemetry is null)
                    throw new InputParsingException("Sensor data found before sensor type declaration.");

                currentSensorTelemetry.Telemetry.Add(new TelemetryRecord(ts, value));
            }
            else
            {
                if (currentSensorTelemetry != null)
                    yield return currentSensorTelemetry;
                currentSensorTelemetry = new SensorTelemetry(parts[0], parts[1], []);
            }
        }

        if (currentSensorTelemetry != null)
            yield return currentSensorTelemetry;
    }

    public void Dispose()
    {
        streamReader.Dispose();
        GC.SuppressFinalize(this);
    }

    private KeyValuePair<string, double> ParseReferenceValue(string value, int index)
    {
        var sensorType = referenceDataOptions.Keys[index];
        if (!double.TryParse(value, CultureInfo.InvariantCulture, out var parsedValue))
            throw new InputParsingException($"Failed to parse a reference value: {value}");

        return new KeyValuePair<string, double>(sensorType, parsedValue);
    }

    private static bool IsSensorData(string[] parts, out DateTime timestamp, out double value)
    {
        value = default;

        return DateTime.TryParse(parts[0], out timestamp) &&
               double.TryParse(parts[1], CultureInfo.InvariantCulture, out value);
    }
}
namespace SensorsQualityEvaluation.Models;

public record SensorTelemetry(string SensorType, string SensorName, List<TelemetryRecord> Telemetry);
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SensorsQualityEvaluation;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddLogging(b => b.AddSimpleConsole())
    .AddSensorQualityEvaluation(o => configuration.GetSection("ReferenceData").Bind(o));


var serviceProvider = serviceCollection.BuildServiceProvider();
var evaluator = serviceProvider.GetRequiredService<ILogFileEvaluator>();

await evaluator.Evaluate("TelemetryInput.txt", "EvaluationOutput.json");
Console.ReadKey();
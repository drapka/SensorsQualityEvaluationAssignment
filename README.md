# SensorsQualityEvaluation

My attempt to complete the assignment. 

The repository consists of three parts:
* **SensorsQualityEvaluation**: A library with the core evaluation logic.
* **SensorsQualityEvaluation.App**: A simple console application to demonstrate and test the library.
* **SensorsQualityEvaluation.Tests**: A test project.

## Notes
* The library integrates with the Microsoft.Extensions.DependencyInjection libraries, including the options pattern and logging as I believe this is how it would be used in any enterprise level app. 
* I tried to leave the input and output format unmodified even if the assignment states that we can make some changes to improve the process. But we do not always have an opportunity to do that as the input could be given by a third party API, device protocol, etc. 
* Although I left the input and output formats untouched, I tried to design everything in an extensible way so that more sensors could be added in the future. I also aimed to minimize the amount of data stored in RAM.
  
## Ideas for possible enhancements
* Move some of the constants to options (e.g. evaluation strategies).
* Stream sensor telemetry data to further reduce memory usage.
* Add support for more input parsers and output serializers.
* Generalise evaluation strategies - if more sensor types are added, many can have the same quality conditions.
* Add more unit tests for better code coverage.

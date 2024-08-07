using SensorsQualityEvaluation.Utils;

namespace SensorsQualityEvaluation.Tests;

[TestClass]
public class MathUtilsTests
{
    [TestMethod]
    public void GetStandardDeviation_ValidData_ShouldReturnCorrectMeanAndStandardDeviation()
    {
        var values = new double[] { 46,69,32,60,52,41 };
        var (mean, stdDev) = MathUtils.GetStandardDeviation(values, v => v);

        Assert.AreEqual(50, mean);
        Assert.AreEqual(13.31, stdDev, 0.005);
    }

    [TestMethod]
    public void AllWithinTolerance_AllValuesWithinTolerance_ShouldReturnTrue()
    {
        var values = new[] { 45.0, 45.5, 44.5, 45.2, 44.8 };
        const double referenceValue = 45.0;
        const double tolerance = 1.0;

        var result = MathUtils.AllWithinTolerance(values, v => v, referenceValue, tolerance);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AllWithinTolerance_OneValueOutsideTolerance_ShouldReturnFalse()
    {
        // Arrange
        var values = new[] { 45.0, 45.5, 44.5, 45.2, 43.0 };
        const double referenceValue = 45.0;
        const double tolerance = 1.0;

        var result = MathUtils.AllWithinTolerance(values, v => v, referenceValue, tolerance);

        Assert.IsFalse(result);
    }
}
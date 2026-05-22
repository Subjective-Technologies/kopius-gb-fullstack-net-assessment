using FluentAssertions;
using GbIrl.Core.Enums;
using GbIrl.Core.Services;
using GbIrl.Spec.Tests.Support;

namespace GbIrl.Spec.Tests.Core;

/// <summary>S10–S16: Total OLV and unit-cost statistics.</summary>
public class IrlStatisticsServiceTests
{
    [Fact]
    public void S10_TotalOlv_SumsPerRowOlv()
    {
        var items = TestData.SubsetForFilterTests();
        var expected = items.Sum(i =>
            OlvCalculator.Calculate(i.UnitCost, i.Quantity, i.Condition));

        IrlStatisticsService.TotalOlv(items).Should().Be(expected);
    }

    [Fact]
    public void S11_WeightedAverageUnitCost_WeightsByQuantity()
    {
        var items = TestData.SubsetForFilterTests();
        // (100*10 + 200*2 + 50*4 + 300*1) / (10+2+4+1) = 1900/17
        var expected = 1900m / 17m;

        IrlStatisticsService.WeightedAverageUnitCost(items).Should().Be(expected);
    }

    [Fact]
    public void S12_MedianUnitCost_OddCount_TakesMiddleValue()
    {
        var items = new[]
        {
            TestData.Item(unitCost: 10m, id: 1),
            TestData.Item(unitCost: 50m, id: 2),
            TestData.Item(unitCost: 100m, id: 3),
        };

        IrlStatisticsService.MedianUnitCost(items).Should().Be(50m);
    }

    [Fact]
    public void S13_MedianUnitCost_EvenCount_AveragesTwoMiddleValues()
    {
        var items = new[]
        {
            TestData.Item(unitCost: 10m, id: 1),
            TestData.Item(unitCost: 20m, id: 2),
            TestData.Item(unitCost: 100m, id: 3),
            TestData.Item(unitCost: 200m, id: 4),
        };

        IrlStatisticsService.MedianUnitCost(items).Should().Be(60m);
    }

    [Fact]
    public void S14_StandardDeviationUnitCost_SampleFormula()
    {
        var items = new[]
        {
            TestData.Item(unitCost: 2m, id: 1),
            TestData.Item(unitCost: 4m, id: 2),
            TestData.Item(unitCost: 4m, id: 3),
            TestData.Item(unitCost: 4m, id: 4),
            TestData.Item(unitCost: 5m, id: 5),
            TestData.Item(unitCost: 5m, id: 6),
            TestData.Item(unitCost: 7m, id: 7),
            TestData.Item(unitCost: 9m, id: 8),
        };
        var expected = (decimal)Math.Sqrt(32d / 7d);

        IrlStatisticsService.StandardDeviationUnitCost(items).Should().BeApproximately(expected, 0.0001m);
    }

    [Fact]
    public void S15_StandardDeviationUnitCost_EmptyOrSingle_ReturnsZero()
    {
        IrlStatisticsService.StandardDeviationUnitCost(Array.Empty<GbIrl.Core.Entities.IrlItem>())
            .Should().Be(0m);
        IrlStatisticsService.StandardDeviationUnitCost([TestData.Item(unitCost: 42m)])
            .Should().Be(0m);
    }

    [Fact]
    public void S16_FilteredSubset_RecomputesAllAggregates()
    {
        var all = TestData.SubsetForFilterTests();
        var electronicsOnly = all.Where(i => i.Category == "Electronics").ToList();

        IrlStatisticsService.TotalOlv(electronicsOnly).Should()
            .BeLessThan(IrlStatisticsService.TotalOlv(all));
        IrlStatisticsService.WeightedAverageUnitCost(electronicsOnly).Should()
            .NotBe(IrlStatisticsService.WeightedAverageUnitCost(all));
    }
}

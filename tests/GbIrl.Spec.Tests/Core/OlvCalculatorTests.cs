using FluentAssertions;
using GbIrl.Core.Enums;
using GbIrl.Core.Services;

namespace GbIrl.Spec.Tests.Core;

/// <summary>S01–S06: OLV formula and condition factors.</summary>
public class OlvCalculatorTests
{
    [Theory]
    [InlineData(ItemCondition.New, 0.70)]
    [InlineData(ItemCondition.Used, 0.50)]
    [InlineData(ItemCondition.Damaged, 0.20)]
    public void S0x_ConditionFactor_MatchesSpec(ItemCondition condition, decimal expectedFactor)
    {
        OlvCalculator.GetConditionFactor(condition).Should().Be(expectedFactor);
    }

    [Fact]
    public void S04_Olv_NewItem_Uses70PercentFactor()
    {
        OlvCalculator.Calculate(100m, 10, ItemCondition.New).Should().Be(700m);
    }

    [Fact]
    public void S05_Olv_UsedItem_Uses50PercentFactor()
    {
        OlvCalculator.Calculate(200m, 3, ItemCondition.Used).Should().Be(300m);
    }

    [Fact]
    public void S06_Olv_DamagedItem_Uses20PercentFactor()
    {
        OlvCalculator.Calculate(50m, 4, ItemCondition.Damaged).Should().Be(40m);
    }
}

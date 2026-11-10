using RefLab.Domain.ClinicalKnowledge;
using RefLab.Domain.Rules;

namespace RefLab.Domain.Tests.Rules;

/// <summary>
/// The clinical constants are resolved from the configured groups at run time.
/// </summary>
public class ClinicalConstantResolverTests
{
    private static readonly ClinicalConstant[] Groups =
    {
        ClinicalConstant.Create("CBCCONS", "3.3", "6.25", "6.75", "7.5", "8.25"),
        ClinicalConstant.Create("PTCONS", "12", "35", "1.33", "2CVJW"),
    };

    [Fact]
    public void A_group_is_resolved_by_its_name()
    {
        var group = ClinicalConstantResolver.Resolve(Groups, "CBCCONS");

        Assert.NotNull(group);
        Assert.Equal("CBCCONS", group!.GroupName);
    }

    [Fact]
    public void The_group_name_is_matched_without_regard_to_case()
    {
        var group = ClinicalConstantResolver.Resolve(Groups, "cbccons");

        Assert.NotNull(group);
    }

    [Fact]
    public void An_unconfigured_group_resolves_to_nothing()
    {
        Assert.Null(ClinicalConstantResolver.Resolve(Groups, "MISSING"));
    }

    [Fact]
    public void A_numeric_constant_is_resolved()
    {
        var value = ClinicalConstantResolver.ResolveNumber(Groups, "CBCCONS", 1);

        Assert.Equal(3.3m, value);
    }

    [Fact]
    public void A_non_numeric_constant_is_resolved_as_text()
    {
        var text = ClinicalConstantResolver.ResolveText(Groups, "PTCONS", 4);

        Assert.Equal("2CVJW", text);
    }

    [Fact]
    public void A_missing_group_raises_a_domain_exception()
    {
        Assert.Throws<InvalidClinicalConstantException>(() =>
            ClinicalConstantResolver.ResolveNumber(Groups, "MISSING", 1));
    }
}

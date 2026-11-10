using RefLab.Domain.Identity;

namespace RefLab.Domain.Tests.Identity;

/// <summary>
/// The legacy operator table carries seventeen permission bits. One of them
/// governed the digital delivery of results, a capability permanently removed
/// from scope, so Ref Lab defines fifteen named capabilities.
/// </summary>
public class PermissionTests
{
    [Fact]
    public void Exactly_fifteen_permissions_are_defined()
    {
        var count = Enum.GetValues<Permission>().Length;

        Assert.Equal(15, count);
    }

    [Fact]
    public void Every_permission_has_a_distinct_value()
    {
        var values = Enum.GetValues<Permission>().Select(value => (int)value).ToArray();

        Assert.Equal(values.Length, values.Distinct().Count());
    }
}

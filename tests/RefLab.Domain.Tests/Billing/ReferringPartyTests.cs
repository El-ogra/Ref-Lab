using RefLab.Domain.Billing;
using RefLab.Domain.Exceptions;
using RefLab.Domain.ValueObjects;

namespace RefLab.Domain.Tests.Billing;

/// <summary>
/// A referring party holds the commission rate used by the accounting module.
/// </summary>
public class ReferringPartyTests
{
    [Fact]
    public void Create_stores_the_identity()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic One");

        Assert.Equal(19, party.Id);
        Assert.Equal("CL-01", party.Code);
        Assert.Equal("Clinic One", party.Name);
    }

    [Fact]
    public void Create_rejects_an_empty_code()
    {
        Assert.Throws<InvalidPaymentException>(() => ReferringParty.Create(19, "  ", "Clinic"));
    }

    [Fact]
    public void Create_rejects_an_empty_name()
    {
        Assert.Throws<InvalidPaymentException>(() => ReferringParty.Create(19, "CL-01", "  "));
    }

    [Fact]
    public void The_commission_rate_is_recorded()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic");

        party.SetCommissionRate(Percentage.Create(10m));

        Assert.Equal(10m, party.CommissionRate!.Value);
    }

    [Fact]
    public void The_special_treatment_is_recorded()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic");

        party.SetSpecialTreatment(true, 500m);

        Assert.True(party.IsSpecial);
        Assert.Equal(500m, party.SpecialLimit);
    }

    [Fact]
    public void The_treating_physician_split_is_recorded()
    {
        var party = ReferringParty.Create(19, "CL-01", "Clinic");

        party.SetTreatingPhysician("PW-9", physicianId: 7);

        Assert.Equal("PW-9", party.TreatingPhysicianCode);
        Assert.Equal(7, party.TreatingPhysicianId);
    }
}

using System.Reflection;

namespace RefLab.Application.Tests.Architecture;

/// <summary>
/// Guards the Clean Architecture dependency rule for the Application layer.
/// The Application layer may reference MediatR, FluentValidation and the
/// Domain layer, but it must not know about a persistence engine or the WPF
/// presentation stack.
/// </summary>
public class ApplicationLayerBoundaryTests
{
    private static IReadOnlyList<string> GetReferencedAssemblyNames(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        return assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .ToArray();
    }

    [Fact]
    public void Application_assembly_is_deployed_to_the_test_output()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "RefLab.Application.dll");

        Assert.True(File.Exists(path), $"Expected {path} to exist.");
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.EntityFrameworkCore.Relational")]
    [InlineData("Microsoft.EntityFrameworkCore.SqlServer")]
    [InlineData("Microsoft.Data.SqlClient")]
    [InlineData("PresentationFramework")]
    [InlineData("WindowsBase")]
    public void Application_does_not_reference_persistence_or_presentation_frameworks(string forbiddenAssembly)
    {
        var referenced = GetReferencedAssemblyNames("RefLab.Application");

        Assert.DoesNotContain(forbiddenAssembly, referenced);
    }

    [Fact]
    public void Application_does_not_reference_the_infrastructure_layer()
    {
        var referenced = GetReferencedAssemblyNames("RefLab.Application");

        Assert.DoesNotContain("RefLab.Infrastructure", referenced);
    }

    [Fact]
    public void Application_references_the_domain_layer()
    {
        var referenced = GetReferencedAssemblyNames("RefLab.Application");

        Assert.Contains("RefLab.Domain", referenced);
    }
}

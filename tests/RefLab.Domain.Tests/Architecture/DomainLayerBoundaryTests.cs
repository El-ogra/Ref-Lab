using System.Reflection;

namespace RefLab.Domain.Tests.Architecture;

/// <summary>
/// Guards the Clean Architecture dependency rule for the Domain layer.
/// The Domain layer must stay free of any framework dependency.
/// </summary>
public class DomainLayerBoundaryTests
{
    private static IReadOnlyList<string> GetReferencedAssemblyNames(string assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        return assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .ToArray();
    }

    [Fact]
    public void Domain_assembly_is_deployed_to_the_test_output()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "RefLab.Domain.dll");

        Assert.True(File.Exists(path), $"Expected {path} to exist.");
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.EntityFrameworkCore.Relational")]
    [InlineData("Microsoft.EntityFrameworkCore.SqlServer")]
    [InlineData("Microsoft.Data.SqlClient")]
    [InlineData("MediatR")]
    [InlineData("FluentValidation")]
    [InlineData("PresentationFramework")]
    [InlineData("WindowsBase")]
    [InlineData("Microsoft.Extensions.DependencyInjection.Abstractions")]
    public void Domain_does_not_reference_framework_dependencies(string forbiddenAssembly)
    {
        var referenced = GetReferencedAssemblyNames("RefLab.Domain");

        Assert.DoesNotContain(forbiddenAssembly, referenced);
    }

    [Fact]
    public void Domain_references_only_the_base_class_library()
    {
        var referenced = GetReferencedAssemblyNames("RefLab.Domain");

        var thirdParty = referenced
            .Where(name => !name.StartsWith("System", StringComparison.Ordinal))
            .Where(name => !string.Equals(name, "netstandard", StringComparison.Ordinal))
            .Where(name => !string.Equals(name, "mscorlib", StringComparison.Ordinal))
            .Where(name => !string.Equals(name, "Microsoft.CSharp", StringComparison.Ordinal))
            .ToArray();

        Assert.True(
            thirdParty.Length == 0,
            $"The Domain layer must not reference third-party assemblies. Found: {string.Join(", ", thirdParty)}.");
    }
}

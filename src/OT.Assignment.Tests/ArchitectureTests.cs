using NetArchTest.Rules;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Domain.Entities;
using OT.Assignment.Infrastructure;

namespace OT.Assignment.Tests;

public class ArchitectureTests
{
    private const string DomainNamespace = "OT.Assignment.Domain";
    private const string ApplicationNamespace = "OT.Assignment.Application";
    private const string InfrastructureNamespace = "OT.Assignment.Infrastructure";

    [Fact]
    public void ApplicationLayer_ShouldNotDependOnInfrastructureLayer()
    {
        var result = Types.InAssembly(typeof(IWagerService).Assembly)
            .That()
            .ResideInNamespaceStartingWith(ApplicationNamespace)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void InfrastructureLayer_ShouldDependOnApplicationLayer()
    {
        var result = Types.InAssembly(typeof(RabbitMqWagerBackgroundService).Assembly)
            .That()
            .ResideInNamespaceStartingWith(InfrastructureNamespace)
            // Since models are typically bare
            .And().DoNotResideInNamespaceContaining("Models")
            .Should()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void DomainLayer_ShouldNotDependOnApplicationOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(Game).Assembly)
            .That()
            .ResideInNamespaceStartingWith(DomainNamespace)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void EntitiesShouldImplementIBaseEntityInterface()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(("OT.Assignment.Domain.Entities"))
            .And().AreClasses()
            .And().DoNotHaveName(nameof(BaseEntity))
            .Should().Inherit(typeof(BaseEntity))
            .GetResult();
        Assert.True(result.IsSuccessful);
    }
}
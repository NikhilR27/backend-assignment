using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OT.Assignment.Infrastructure.IoC;

public class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // TODO centralize injection of deps
    }
}
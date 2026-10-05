using Application.Common.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigurePersistenceServices
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddRepositories();
    }
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IPetPatientRepository, PetPatientRepository>();
    }
}
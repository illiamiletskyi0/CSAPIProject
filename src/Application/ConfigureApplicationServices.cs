using Application.PetPatients.Services.Implementation;
using Application.PetPatients.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPetPatientService, PetPatientService>();
    }
}
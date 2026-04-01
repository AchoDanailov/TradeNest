using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TradeNest.Web.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection RegisterUserServices(this IServiceCollection services,
        Assembly assembly)
    {
        IEnumerable<Type> servicesInterfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Service"));
        foreach (Type serviceInterface in servicesInterfaces)
        {
            Type? serviceImplementation = assembly.GetExportedTypes()
                .SingleOrDefault(t => 
                    t.IsAssignableTo(serviceInterface) && t is { IsAbstract: false, IsClass: true }
                                                       && t.Name.EndsWith("Service"));
            if (serviceImplementation == null)
                continue;

            services.AddScoped(serviceInterface, serviceImplementation);
        }

        return services;
    }
    
    public static IServiceCollection RegisterRepositories(this IServiceCollection services,
        Assembly assembly)
    {
        IEnumerable<Type> repositoriesInterfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Repository") 
                        && !t.IsGenericType);
        foreach (Type repositoryInterface in repositoriesInterfaces)
        {
            Type? repositoryImplementation = assembly.GetExportedTypes()
                .SingleOrDefault(t => 
                    t.IsAssignableTo(repositoryInterface) && t is { IsAbstract: false, IsClass: true }
                                                          && t.Name.EndsWith("Repository"));
            if (repositoryImplementation == null)
                continue;

            services.AddScoped(repositoryInterface, repositoryImplementation);
        }

        return services;
    }

    public static IServiceCollection RegisterMappings(this IServiceCollection services,
        Assembly assembly, params Assembly[] assemblies)
    {
        services = ProcessMapperAssembly(services, assembly);

        foreach (Assembly extraAssembly in assemblies)
            services = ProcessMapperAssembly(services, extraAssembly);

        return services;
    }
    
    public static IServiceCollection RegisterSeeders(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> seedersInterfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Seeder") &&
                        !t.Name.Contains("Entity") && !t.IsGenericType);
        foreach (Type seederInterface in seedersInterfaces)
        {
            Type? seederImplType = assembly.GetExportedTypes()
                .SingleOrDefault(t => 
                    t.IsAssignableTo(seederInterface) && t is { IsAbstract: false, IsClass: true }
                                                      && t.Name.EndsWith("Seeder"));
            if (seederImplType == null)
                continue;

            services.AddScoped(seederInterface, seederImplType);
        }

        return services;
    }

    private static IServiceCollection ProcessMapperAssembly(IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> mappersInterfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I") && t.Name.EndsWith("Mapper")
                        && !t.IsGenericType);
        foreach (Type mapperInterface in mappersInterfaces)
        {
            Type? mapperImplementation = assembly.GetExportedTypes()
                .SingleOrDefault(t => 
                    t.IsAssignableTo(mapperInterface) && t is { IsAbstract: false, IsClass: true }
                                                      && t.Name.EndsWith("Mapper"));
            if (mapperImplementation == null)
                continue;

            services.AddSingleton(mapperInterface, mapperImplementation);
        }

        return services;
    }
}
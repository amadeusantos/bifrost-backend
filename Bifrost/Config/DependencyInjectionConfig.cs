using Bifrost.Core.Adapter;
using Bifrost.Core.Port.Gateway;
using Bifrost.Core.Port.Repository;
using Bifrost.Core.Service;
using Bifrost.Infrastructure.Gateway.OAuth;
using Bifrost.Infrastructure.Persistence;
using Bifrost.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Config;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddTransient<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseService, CourseService>();

        services.AddTransient<IAssessmentSeasonRepository, AssessmentSeasonRepository>();
        services.AddScoped<IAssessmentSeasonService, AssessmentSeasonService>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddTransient<ICoordinationRepository, CoordinationRepository>();
        services.AddScoped<ICoordinationService, CoordinationService>();

        services.AddTransient<IAcademicCenterRepository, AcademicCenterRepository>();
        services.AddScoped<IAcademicCenterService, AcademicCenterService>();

        services.AddTransient<IDisciplineRepository, DisciplineRepository>();
        services.AddScoped<IDisciplineService, DisciplineService>();

        services.AddHttpClient<IOAuthGateway, OAuthGateway>();
    }
}

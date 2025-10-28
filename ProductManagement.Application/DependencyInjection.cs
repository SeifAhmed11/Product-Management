using Microsoft.Extensions.DependencyInjection;

namespace ProductManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // مساحة لإضافة خدمات التطبيق مستقبلاً (MediatR, Validators, Mappers)
            return services;
        }
    }
}



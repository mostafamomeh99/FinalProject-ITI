//using IRepositoryService;
//using Models;
//using RepositoryFactory;

//namespace WebApplication1.Extensions
//{
//    public static class StripeServiceExtensions
//    {
//        public static IServiceCollection AddStripeInfrastructure(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
//            services.AddScoped<IStripeService, StripeService>();
//            return services;
//        }
//    }
//}

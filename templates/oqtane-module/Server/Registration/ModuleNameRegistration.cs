using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using RootNamespace.Repository;

namespace RootNamespace
{
    public class ModuleNameRegistration : IServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IModuleNameRepository, ModuleNameRepository>();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider) { }
    }
}

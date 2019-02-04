using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class GISSMHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "GISSM";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(GISSMApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}

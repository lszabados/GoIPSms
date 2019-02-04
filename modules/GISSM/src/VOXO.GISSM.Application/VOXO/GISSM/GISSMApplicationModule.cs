using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMDomainModule),
        typeof(GISSMApplicationContractsModule),
        typeof(AbpAutoMapperModule)
        )]
    public class GISSMApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<GISSMApplicationAutoMapperProfile>(validate: true);
            });

            Configure<SettingOptions>(options =>
            {
                options.DefinitionProviders.Add<GISSMSettingDefinitionProvider>();
            });
        }
    }
}

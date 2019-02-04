using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using VOXO.GISSM.Localization;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(AbpLocalizationModule)
        )]
    public class GISSMDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources.Add<GISSMResource>("en");
            });
        }
    }
}

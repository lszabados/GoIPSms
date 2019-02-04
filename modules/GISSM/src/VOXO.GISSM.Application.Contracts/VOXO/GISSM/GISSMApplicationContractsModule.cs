using Microsoft.Extensions.DependencyInjection;
using VOXO.GISSM.Localization;
using Volo.Abp.Application;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMDomainSharedModule),
        typeof(AbpDddApplicationModule)
        )]
    public class GISSMApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<PermissionOptions>(options =>
            {
                options.DefinitionProviders.Add<GISSMPermissionDefinitionProvider>();
            });

            Configure<VirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<GISSMApplicationContractsModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<GISSMResource>()
                    .AddVirtualJson("/VOXO/GISSM/Localization/ApplicationContracts");
            });
        }
    }
}

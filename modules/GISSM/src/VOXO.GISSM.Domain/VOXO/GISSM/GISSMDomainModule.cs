using Microsoft.Extensions.DependencyInjection;
using VOXO.GISSM.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMDomainSharedModule)
        )]
    public class GISSMDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<VirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<GISSMDomainModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources.Get<GISSMResource>().AddVirtualJson("/VOXO/GISSM/Localization/Domain");
            });

            Configure<ExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("GISSM", typeof(GISSMResource));
            });
        }
    }
}

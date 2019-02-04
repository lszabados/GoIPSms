using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class GISSMHttpApiModule : AbpModule
    {
        
    }
}

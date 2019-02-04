using VOXO.GISSM.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMEntityFrameworkCoreTestModule)
        )]
    public class GISSMDomainTestModule : AbpModule
    {
        
    }
}

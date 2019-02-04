using Volo.Abp.Modularity;

namespace VOXO.GISSM
{
    [DependsOn(
        typeof(GISSMApplicationModule),
        typeof(GISSMDomainTestModule)
        )]
    public class GISSMApplicationTestModule : AbpModule
    {

    }
}

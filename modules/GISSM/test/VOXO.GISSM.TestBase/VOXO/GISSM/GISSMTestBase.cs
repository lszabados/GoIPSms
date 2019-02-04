using Volo.Abp;
using Volo.Abp.Modularity;

namespace VOXO.GISSM
{
    public abstract class GISSMTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule> 
        where TStartupModule : IAbpModule
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}

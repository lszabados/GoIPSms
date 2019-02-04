using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Xunit;

namespace VOXO.GISSM
{
    public abstract class MyEntityRepository_Tests<TStartupModule> : GISSMTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        [Fact]
        public async Task Test1()
        {

        }
    }
}

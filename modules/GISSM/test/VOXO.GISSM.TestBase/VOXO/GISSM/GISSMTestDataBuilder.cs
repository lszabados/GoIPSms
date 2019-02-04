using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace VOXO.GISSM
{
    public class GISSMTestDataBuilder : ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private GISSMTestData _testData;

        public GISSMTestDataBuilder(
            IGuidGenerator guidGenerator,
            GISSMTestData testData)
        {
            _guidGenerator = guidGenerator;
            _testData = testData;
        }

        public void Build()
        {
            
        }
    }
}
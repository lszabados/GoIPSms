using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace VOXO.GISSM.MongoDB
{
    public class GISSMMongoModelBuilderConfigurationOptions : MongoModelBuilderConfigurationOptions
    {
        public GISSMMongoModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = GISSMConsts.DefaultDbTablePrefix)
            : base(tablePrefix)
        {
        }
    }
}
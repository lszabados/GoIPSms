using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace VOXO.GISSM.EntityFrameworkCore
{
    public class GISSMModelBuilderConfigurationOptions : ModelBuilderConfigurationOptions
    {
        public GISSMModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = GISSMConsts.DefaultDbTablePrefix,
            [CanBeNull] string schema = GISSMConsts.DefaultDbSchema)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}
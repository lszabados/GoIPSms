using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace VOXO.GISSM.MongoDB
{
    public static class GISSMMongoDbContextExtensions
    {
        public static void ConfigureGISSM(
            this IMongoModelBuilder builder,
            Action<MongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new GISSMMongoModelBuilderConfigurationOptions();

            optionsAction?.Invoke(options);
        }
    }
}
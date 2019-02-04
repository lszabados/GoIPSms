using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace VOXO.GISSM.MongoDB
{
    [ConnectionStringName("GISSM")]
    public class GISSMMongoDbContext : AbpMongoDbContext, IGISSMMongoDbContext
    {
        public static string CollectionPrefix { get; set; } = GISSMConsts.DefaultDbTablePrefix;

        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureGISSM(options =>
            {
                options.CollectionPrefix = CollectionPrefix;
            });
        }
    }
}
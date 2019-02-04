using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace VOXO.GISSM.MongoDB
{
    [ConnectionStringName("GISSM")]
    public interface IGISSMMongoDbContext : IAbpMongoDbContext
    {
        /* Define mongo collections here. Example:
         * IMongoCollection<Question> Questions { get; }
         */
    }
}

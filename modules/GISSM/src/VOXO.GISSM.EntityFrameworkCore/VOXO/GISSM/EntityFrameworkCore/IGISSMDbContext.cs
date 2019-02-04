using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VOXO.GISSM.EntityFrameworkCore
{
    [ConnectionStringName("GISSM")]
    public interface IGISSMDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace VOXO.GISSM.EntityFrameworkCore
{
    [ConnectionStringName("GISSM")]
    public class GISSMDbContext : AbpDbContext<GISSMDbContext>, IGISSMDbContext
    {
        public static string TablePrefix { get; set; } = GISSMConsts.DefaultDbTablePrefix;

        public static string Schema { get; set; } = GISSMConsts.DefaultDbSchema;

        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public GISSMDbContext(DbContextOptions<GISSMDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureGISSM(options =>
            {
                options.TablePrefix = TablePrefix;
                options.Schema = Schema;
            });
        }
    }
}
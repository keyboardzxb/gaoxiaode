using Microsoft.EntityFrameworkCore;

namespace Arrowgene.O2Jam.Server.Data
{
    public class O2JamDbContext : DbContext
    {
        public O2JamDbContext(DbContextOptions<O2JamDbContext> options) : base(options) { }

        public DbSet<UserEntity> Accounts { get; set; }
        public DbSet<PlayerEntity> Characters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 显式配置表名
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Accounts");
            });

            modelBuilder.Entity<PlayerEntity>(entity =>
            {
                entity.ToTable("Characters");
            });
        }
    }
}
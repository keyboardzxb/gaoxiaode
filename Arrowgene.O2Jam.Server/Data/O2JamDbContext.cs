using Microsoft.EntityFrameworkCore;

namespace Arrowgene.O2Jam.Server.Data
{
    public class O2JamDbContext : DbContext
    {
        public O2JamDbContext(DbContextOptions<O2JamDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<MemberEntity> Members { get; set; }
        public DbSet<CashEntity> Cashes { get; set; }
        public DbSet<LoadCharSpDto> LoadCharSpDtos { get; set; } // For mapping SP results

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure LoadCharSpDto as a keyless entity type because it's not a real table
            modelBuilder.Entity<LoadCharSpDto>().HasNoKey();
        }
    }
}
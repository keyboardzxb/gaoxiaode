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
    }
}
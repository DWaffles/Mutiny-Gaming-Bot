using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MutinyBot.Entities;
using System;
using System.IO;
using System.Linq;

namespace MutinyBot.Database
{
    public class MutinyBotDbContext : DbContext
    {
        public DbSet<GuildEntity> Guilds { get; set; }
        public DbSet<MemberEntity> Members { get; set; }
        public MutinyBot MutinyBot { protected get; set; }
        public MutinyBotDbContext()
        {
            if (Database.GetPendingMigrations().Any())
                Database.Migrate();
            else
                Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("DATABASE");

            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
            Console.WriteLine(dbPath);
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            string dbFilePath = Path.Combine(dbPath, "MutinyBotDatabase.sqlite.db");


            _ = optionsBuilder.UseSqlite($"Filename={dbFilePath}");
            _ = optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //GuildEntity
            _ = modelBuilder.Entity<GuildEntity>().HasIndex(x => x.GuildId).IsUnique();
            _ = modelBuilder.Entity<GuildEntity>().Property(x => x.GuildId).IsRequired();

            //MemberEntity
            _ = modelBuilder.Entity<MemberEntity>().HasIndex(x => new { x.MemberId, x.GuildId }).IsUnique(true);
            _ = modelBuilder.Entity<MemberEntity>().Property(x => x.RoleDictionary).HasJsonConversion();
        }
    }
}

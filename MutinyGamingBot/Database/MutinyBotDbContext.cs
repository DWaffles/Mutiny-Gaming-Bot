using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MutinyBot.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MutinyBot.Database
{
    public class MutinyBotDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<GuildModel> Guilds { get; set; }
        public DbSet<MemberModel> Members { get; set; }
        private static ILoggerFactory LoggerFactory => new LoggerFactory().AddSerilog();
        public MutinyBotDbContext() { }

        /// <summary>
        /// Applies pending database migrations to the current database, and will create one if it does not already exist.
        /// </summary>
        /// <remarks>
        /// Reference <see href="https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs">Database migrations</see> for information.
        /// </remarks>
        /// 
        public void ApplyMigrations()
        {
            if (Database.GetPendingMigrations().Any())
            {
                var numMigrations = Database.GetPendingMigrations().Count();
                Log.Information($"[DATABASE] Applying {numMigrations} migrations to the database.");
                Database.Migrate();
                SaveChanges();
                Log.Information($"[DATABASE] {numMigrations} migrations have been applied to the database.");
            }
            else
            {
                Log.Information($"[DATABASE] No pending migrations, database is up to date.");
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DirectoryInfo dir = Directory.CreateDirectory("data");
            string dbFilePath = Path.Combine(dir.Name, "MutinyBotDatabase.sqlite.db");

            optionsBuilder.UseLoggerFactory(LoggerFactory);

            optionsBuilder.UseSqlite($"Filename={dbFilePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var listStringValueComparer = new ValueComparer<IEnumerable<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c);

            var dictionaryUlongBoolValueComparer = new ValueComparer<Dictionary<ulong, bool>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(m => m.UserId);
                entity.Property(m => m.UserId).ValueGeneratedNever();
            });
            modelBuilder.Entity<GuildModel>(entity =>
            {
                entity.HasKey(m => m.GuildId);
                entity.Property(m => m.GuildId).ValueGeneratedNever();
                entity.HasMany(m => m.Members).WithOne(m => m.Guild).HasForeignKey(m => m.GuildId).IsRequired();
            });
            modelBuilder.Entity<MemberModel>(entity =>
            {
                entity.HasKey(g => new { g.MemberId, g.GuildId });
                entity.Ignore(entity => entity.LastMessageTimestamp);
                entity.Property(entity => entity.LastMessageTimestampRaw);
                entity.Property(entity => entity.RoleDictionary)
                    .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<ulong, bool>>(v),
                    dictionaryUlongBoolValueComparer);
            });
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MutinyBot.Entities;
using System;
using System.IO;
using System.Linq;

namespace MutinyBot.Database
{
    public class MutinyBotDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GuildEntity> Guilds { get; set; }
        public DbSet<MemberEntity> Members { get; set; }
        public MutinyBot MutinyBot { protected get; set; }
        public MutinyBotDbContext() 
        {
            if(Database.GetPendingMigrations().Any())
                Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("DATABASE");

            string dbPath = Path.Combine(AppContext.BaseDirectory, "data");
            Console.WriteLine(dbPath);
            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }
            string dbFilePath = Path.Combine(dbPath, "MutinyBotDatabase.sqlite.db");

            _ = optionsBuilder.UseSqlite($"Filename={dbFilePath}");
            _ = optionsBuilder.LogTo(Console.WriteLine, LogLevel.Warning);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //UserEntity
            _ = modelBuilder.Entity<UserEntity>().HasIndex(x => x.UserId).IsUnique();
            _ = modelBuilder.Entity<UserEntity>().Property(x => x.UserId).IsRequired();

            //GuildEntity
            _ = modelBuilder.Entity<GuildEntity>().HasIndex(x => x.GuildId).IsUnique();
            _ = modelBuilder.Entity<GuildEntity>().Property(x => x.GuildId).IsRequired();

            //MemberEntity
            _ = modelBuilder.Entity<MemberEntity>().HasIndex(x => new { x.MemberId, x.GuildId }).IsUnique(true);
            _ = modelBuilder.Entity<MemberEntity>().Property(x => x.RoleDictionary).HasJsonConversion();
        }
    }
}

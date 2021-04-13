﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MutinyBot.Database;

namespace MutinyBot.Migrations
{
    [DbContext(typeof(MutinyBotDbContext))]
    [Migration("20210413111410_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("MutinyBot.Entities.GuildEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GuildName")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("JoinLogChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("JoinLogEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MuteRoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GuildId")
                        .IsUnique();

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("MutinyBot.Entities.MemberEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CurrentMember")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MemberId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RoleDictionary")
                        .HasColumnType("TEXT");

                    b.Property<int>("TimesJoined")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MemberId", "GuildId")
                        .IsUnique();

                    b.ToTable("Members");
                });

            modelBuilder.Entity("MutinyBot.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Banned")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserEntity");
                });
#pragma warning restore 612, 618
        }
    }
}

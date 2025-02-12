﻿// <auto-generated />
using System;
using MailBot.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MailBot.Migrations
{
    [DbContext(typeof(MailBotDbContext))]
    partial class MailBotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MailBot.Models.User.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid[]>("UserMailId")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.Property<long>("chatId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MailBot.Models.User.UserMail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserMails");
                });

            modelBuilder.Entity("MailBot.Models.User.UserMail", b =>
                {
                    b.HasOne("MailBot.Models.User.User", null)
                        .WithMany("UserMails")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MailBot.Models.User.User", b =>
                {
                    b.Navigation("UserMails");
                });
#pragma warning restore 612, 618
        }
    }
}

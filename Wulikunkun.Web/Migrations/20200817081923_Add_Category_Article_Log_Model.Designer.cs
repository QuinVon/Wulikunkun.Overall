﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wulikunkun.Web.Models;

namespace Web.Migrations
{
    [DbContext(typeof(WangKunDbContext))]
    [Migration("20200817081923_Add_Category_Article_Log_Model")]
    partial class Add_Category_Article_Log_Model
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Wulikunkun.Web.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("PublishTime")
                        .HasColumnType("datetime");

                    b.Property<byte>("Tag")
                        .HasColumnType("tinyint");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Wulikunkun.Web.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Wulikunkun.Web.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("LogTime")
                        .HasColumnType("datetime");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Wulikunkun.Web.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<sbyte>("Age")
                        .HasColumnType("smallint");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<int>("Phone")
                        .HasColumnType("int");

                    b.Property<string>("Province")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<DateTime>("RegisterTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("School")
                        .HasColumnType("varchar(256)")
                        .HasMaxLength(256);

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Wulikunkun.Web.Models.Article", b =>
                {
                    b.HasOne("Wulikunkun.Web.Models.Category", "Category")
                        .WithMany("Articles")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Wulikunkun.Web.Models.User", "User")
                        .WithMany("Articles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Wulikunkun.Web.Models.Log", b =>
                {
                    b.HasOne("Wulikunkun.Web.Models.User", "User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SqlBundle.Models;

#nullable disable

namespace SqlBundle.Migrations
{
    [DbContext(typeof(SqlContext))]
    [Migration("20220404163043_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SqlBundle.Models.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Date")
                        .HasColumnType("text");

                    b.Property<string>("Parametrs")
                        .HasColumnType("text");

                    b.Property<string>("Results")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("SqlBundle.Models.HistoryLogger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Date")
                        .HasColumnType("text");

                    b.Property<string>("StateAndExeption")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LogHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
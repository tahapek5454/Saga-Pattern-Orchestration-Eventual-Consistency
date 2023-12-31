﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Stock.API.Context;

#nullable disable

namespace Stock.API.Migrations
{
    [DbContext(typeof(StockDbContext))]
    [Migration("20231206194527_mig-1")]
    partial class mig1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Stock.API.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Stocks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Count = 10,
                            ProductId = 1
                        },
                        new
                        {
                            Id = 2,
                            Count = 10,
                            ProductId = 2
                        },
                        new
                        {
                            Id = 3,
                            Count = 10,
                            ProductId = 3
                        },
                        new
                        {
                            Id = 4,
                            Count = 10,
                            ProductId = 4
                        },
                        new
                        {
                            Id = 5,
                            Count = 10,
                            ProductId = 5
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

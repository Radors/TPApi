﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TPApi.Data;

#nullable disable

namespace TPApi.Migrations
{
    [DbContext(typeof(TPDbContext))]
    [Migration("20240823055119_AddedWeightColumnToFoodProduct")]
    partial class AddedWeightColumnToFoodProduct
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.7.24405.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TPApi.Food.DBModels.FoodEmbedding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OldId")
                        .HasColumnType("int");

                    b.Property<string>("Vector")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FoodEmbeddings");
                });

            modelBuilder.Entity("TPApi.Food.DBModels.FoodProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("A")
                        .HasColumnType("real");

                    b.Property<float>("B1")
                        .HasColumnType("real");

                    b.Property<float>("B12")
                        .HasColumnType("real");

                    b.Property<float>("B2")
                        .HasColumnType("real");

                    b.Property<float>("B3")
                        .HasColumnType("real");

                    b.Property<float>("B6")
                        .HasColumnType("real");

                    b.Property<float>("B9")
                        .HasColumnType("real");

                    b.Property<float>("C")
                        .HasColumnType("real");

                    b.Property<float>("D")
                        .HasColumnType("real");

                    b.Property<float>("E")
                        .HasColumnType("real");

                    b.Property<float>("Jarn")
                        .HasColumnType("real");

                    b.Property<float>("Jod")
                        .HasColumnType("real");

                    b.Property<float>("Kalcium")
                        .HasColumnType("real");

                    b.Property<float>("Kalium")
                        .HasColumnType("real");

                    b.Property<float>("Magnesium")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OldId")
                        .HasColumnType("int");

                    b.Property<float>("Selen")
                        .HasColumnType("real");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.Property<float>("Zink")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("FoodProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
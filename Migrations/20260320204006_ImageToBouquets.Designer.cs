using System;
using FlowerShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#nullable disable
namespace Software_Design1.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260320204006_ImageToBouquets")]
    partial class ImageToBouquets
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);
            modelBuilder.Entity("FlowerShop.Models.Bouquet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");
                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<string>("OccasionTag")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");
                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");
                    b.HasKey("Id");
                    b.ToTable("Bouquets");
                });
            modelBuilder.Entity("FlowerShop.Models.BouquetFlower", b =>
                {
                    b.Property<int>("BouquetId")
                        .HasColumnType("int");
                    b.Property<int>("FlowerId")
                        .HasColumnType("int");
                    b.Property<int>("Quantity")
                        .HasColumnType("int");
                    b.HasKey("BouquetId", "FlowerId");
                    b.HasIndex("FlowerId");
                    b.ToTable("BouquetFlowers");
                });
            modelBuilder.Entity("FlowerShop.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");
                    b.HasKey("Id");
                    b.ToTable("Categories");
                });
            modelBuilder.Entity("FlowerShop.Models.Florist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("Bio")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");
                    b.Property<string>("ContactEmail")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");
                    b.Property<string>("Specialization")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<int?>("UserId")
                        .HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("UserId");
                    b.ToTable("Florists");
                });
            modelBuilder.Entity("FlowerShop.Models.Flower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");
                    b.Property<string>("Color")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");
                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");
                    b.Property<int?>("FloristId")
                        .HasColumnType("int");
                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");
                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("CategoryId");
                    b.HasIndex("FloristId");
                    b.ToTable("Flowers");
                });
            modelBuilder.Entity("FlowerShop.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<int?>("BouquetId")
                        .HasColumnType("int");
                    b.Property<string>("DeliveryAddress")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");
                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");
                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)");
                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");
                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10,2)");
                    b.Property<int>("UserId")
                        .HasColumnType("int");
                    b.HasKey("Id");
                    b.HasIndex("BouquetId");
                    b.HasIndex("UserId");
                    b.ToTable("Orders");
                });
            modelBuilder.Entity("FlowerShop.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<int>("FlowerId")
                        .HasColumnType("int");
                    b.Property<int>("OrderId")
                        .HasColumnType("int");
                    b.Property<int>("Quantity")
                        .HasColumnType("int");
                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10,2)");
                    b.HasKey("Id");
                    b.HasIndex("FlowerId");
                    b.HasIndex("OrderId");
                    b.ToTable("OrderItems");
                });
            modelBuilder.Entity("FlowerShop.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");
                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");
                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");
                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");
                    b.HasKey("Id");
                    b.ToTable("Users");
                });
            modelBuilder.Entity("FlowerShop.Models.BouquetFlower", b =>
                {
                    b.HasOne("FlowerShop.Models.Bouquet", "Bouquet")
                        .WithMany("BouquetFlowers")
                        .HasForeignKey("BouquetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("FlowerShop.Models.Flower", "Flower")
                        .WithMany("BouquetFlowers")
                        .HasForeignKey("FlowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Bouquet");
                    b.Navigation("Flower");
                });
            modelBuilder.Entity("FlowerShop.Models.Florist", b =>
                {
                    b.HasOne("FlowerShop.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                    b.Navigation("User");
                });
            modelBuilder.Entity("FlowerShop.Models.Flower", b =>
                {
                    b.HasOne("FlowerShop.Models.Category", "Category")
                        .WithMany("Flowers")
                        .HasForeignKey("CategoryId");
                    b.HasOne("FlowerShop.Models.Florist", "Florist")
                        .WithMany("Flowers")
                        .HasForeignKey("FloristId");
                    b.Navigation("Category");
                    b.Navigation("Florist");
                });
            modelBuilder.Entity("FlowerShop.Models.Order", b =>
                {
                    b.HasOne("FlowerShop.Models.Bouquet", "Bouquet")
                        .WithMany("Orders")
                        .HasForeignKey("BouquetId");
                    b.HasOne("FlowerShop.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Bouquet");
                    b.Navigation("User");
                });
            modelBuilder.Entity("FlowerShop.Models.OrderItem", b =>
                {
                    b.HasOne("FlowerShop.Models.Flower", "Flower")
                        .WithMany("OrderItems")
                        .HasForeignKey("FlowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("FlowerShop.Models.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Flower");
                    b.Navigation("Order");
                });
            modelBuilder.Entity("FlowerShop.Models.Bouquet", b =>
                {
                    b.Navigation("BouquetFlowers");
                    b.Navigation("Orders");
                });
            modelBuilder.Entity("FlowerShop.Models.Category", b =>
                {
                    b.Navigation("Flowers");
                });
            modelBuilder.Entity("FlowerShop.Models.Florist", b =>
                {
                    b.Navigation("Flowers");
                });
            modelBuilder.Entity("FlowerShop.Models.Flower", b =>
                {
                    b.Navigation("BouquetFlowers");
                    b.Navigation("OrderItems");
                });
            modelBuilder.Entity("FlowerShop.Models.Order", b =>
                {
                    b.Navigation("OrderItems");
                });
            modelBuilder.Entity("FlowerShop.Models.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}

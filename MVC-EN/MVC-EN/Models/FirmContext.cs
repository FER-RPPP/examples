﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MVC_EN.Models
{
    public partial class FirmContext : DbContext
    {
        public FirmContext(DbContextOptions<FirmContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityId)
                    .IsClustered(false);

                entity.ToTable("City");

                entity.HasIndex(e => e.CityName, "IX_City_Name");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.PostalName).HasMaxLength(50);

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_City_Country");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .IsClustered(false);

                entity.ToTable("Company");

                entity.HasIndex(e => e.CompanyName, "IX_Company_Name");

                entity.HasIndex(e => e.RegistrationNumber, "UIX_Company_RegistrationNumber")
                    .IsUnique();

                entity.Property(e => e.CompanyId).ValueGeneratedNever();

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.CompanyNavigation)
                    .WithOne(p => p.Company)
                    .HasForeignKey<Company>(d => d.CompanyId)
                    .HasConstraintName("FK_Company_Partner");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryCode)
                    .IsClustered(false);

                entity.ToTable("Country");

                entity.HasIndex(e => e.CountryName, "UIX_Country_Name")
                    .IsUnique();

                entity.Property(e => e.CountryCode).HasMaxLength(2);

                entity.Property(e => e.CountryIso3).HasMaxLength(3);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocumentId)
                    .IsClustered(false);

                entity.ToTable("Document");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.DocumentType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Vat)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("VAT");

                entity.HasOne(d => d.Partner)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.PartnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_Partner");

                entity.HasOne(d => d.PreviousDocument)
                    .WithMany(p => p.InversePreviousDocument)
                    .HasForeignKey(d => d.PreviousDocumentId)
                    .HasConstraintName("FK_Document_Document");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .IsClustered(false);

                entity.ToTable("Item");

                entity.HasIndex(e => new { e.DocumentId, e.ProductNumber }, "UIX_Item")
                    .IsUnique();

                entity.Property(e => e.Discount).HasColumnType("decimal(4, 2)");

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_Item_Document");

                entity.HasOne(d => d.ProductNumberNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ProductNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Item_Product");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.HasKey(e => e.PartnerId)
                    .IsClustered(false);

                entity.ToTable("Partner");

                entity.HasIndex(e => e.VatNumber, "UIX_Partner_VatNumber")
                    .IsUnique();

                entity.Property(e => e.PartnerType)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.ResidenceAddress).HasMaxLength(50);

                entity.Property(e => e.ShipmentAddress).HasMaxLength(50);

                entity.Property(e => e.VatNumber).HasMaxLength(50);

                entity.HasOne(d => d.ResidenceCity)
                    .WithMany(p => p.PartnerResidenceCities)
                    .HasForeignKey(d => d.ResidenceCityId)
                    .HasConstraintName("FK_Partner_City_Residence");

                entity.HasOne(d => d.ShipmentCity)
                    .WithMany(p => p.PartnerShipmentCities)
                    .HasForeignKey(d => d.ShipmentCityId)
                    .HasConstraintName("FK_Partner_City_Shipment");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.PersonId)
                    .IsClustered(false);

                entity.ToTable("Person");

                entity.Property(e => e.PersonId).ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.PersonNavigation)
                    .WithOne(p => p.Person)
                    .HasForeignKey<Person>(d => d.PersonId)
                    .HasConstraintName("FK_Person_Partner");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductNumber)
                    .IsClustered(false);

                entity.ToTable("Product");

                entity.HasIndex(e => e.ProductName, "UIX_Product_Name")
                    .IsUnique();

                entity.Property(e => e.ProductNumber).ValueGeneratedNever();

                entity.Property(e => e.PhotoChecksum).HasComputedColumnSql("(checksum([Photo]))", false);

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UnitName)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
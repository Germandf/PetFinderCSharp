using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PetFinder.Models
{
    public partial class PetFinderContext : DbContext
    {
        public PetFinderContext()
        {
        }

        public PetFinderContext(DbContextOptions<PetFinderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnimalType> AnimalTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }


        /* Autogenerado */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AI");

            modelBuilder.Entity<AnimalType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(35);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("Pet");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Photo).HasMaxLength(100);

                entity.HasOne(d => d.AnimalType)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.AnimalTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pet_AnimalType");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pet_City");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.GenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pet_Gender");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

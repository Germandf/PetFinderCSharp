using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PetFinder.Areas.Identity;

#nullable disable

namespace PetFinder.Models
{
    public partial class PetFinderContext : IdentityDbContext<ApplicationUser>
    {
        public PetFinderContext()
        {
        }

        public PetFinderContext(DbContextOptions<PetFinderContext> options)
            : base(options)
        {
        }

        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Pet> Pets { get; set; }

        // Creo los generos por defecto ya que no disponen de un CRUD
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var gender1 = new Gender() { Id = 1, Name = "Macho" };
            var gender2 = new Gender() { Id = 2, Name = "Hembra" };
            modelBuilder.Entity<Gender>().HasData(new Gender[] { gender1, gender2});
            base.OnModelCreating(modelBuilder);
        }
    }
}

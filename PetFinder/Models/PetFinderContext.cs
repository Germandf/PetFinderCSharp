using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Models
{
    public class PetFinderContext: DbContext
    {
        public PetFinderContext(DbContextOptions<PetFinderContext> options) : base(options) { }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}

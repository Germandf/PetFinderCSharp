using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;


namespace PetFinderTests
{
    class PetFinderDbContextFactory 
    {

        private DbContextOptions<PetFinderContext> CreateOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<PetFinderContext>().UseInMemoryDatabase(databaseName).Options;
        }

        public PetFinderContext CreateContext()
        {
            
            var options = CreateOptions(Guid.NewGuid().ToString("N"));
            var context = new PetFinderContext(options);

            var initialGenders = new List<Gender>
            {
                new Gender() {Name = "Macho", SerializedName = "MACHO"},
                new Gender() { Name = "Hembra", SerializedName = "HEMBRA" }
            };

            context.Genders.AddRange(initialGenders); 
            context.SaveChanges();

            return context;
        }

    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PetFinder.Models;

namespace PetFinderTests.Integration
{
    internal class PetFinderDbContextFactory
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
                new() {Name = "Macho", SerializedName = "MACHO"},
                new() {Name = "Hembra", SerializedName = "HEMBRA"}
            };

            context.Genders.AddRange(initialGenders);
            context.SaveChanges();

            return context;
        }
    }
}
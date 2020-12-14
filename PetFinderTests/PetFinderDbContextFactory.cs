using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFinderTests
{
    class PetFinderDbContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<PetFinderContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<PetFinderContext>()
                .UseSqlite(_connection).Options;
        }

        public PetFinderContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new PetFinderContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new PetFinderContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class CityService : ICityService
    {
        private readonly PetFinderContext _context;
        public CityService(PetFinderContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            _context.Cities.Remove(city);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<City>> GetAll()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<City> Get(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task<bool> Insert(City city)
        {
            _context.Cities.Add(city);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Save(City city)
        {
            if (city.Id > 0)
            {
                return await Update(city);
            }
            else
            {
                return await Insert(city);
            }
        }

        public async Task<bool> Update(City city)
        {
            _context.Entry(city).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

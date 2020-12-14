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
            if (IsValidName(city.Name))
            {
                if (await IsNotRepeated(city.Name))
                {
                    _context.Cities.Add(city);
                    return await _context.SaveChangesAsync() > 0;
                }
                else
                {
                    throw new CityAlreadyExistsException("Ya existe una ciudad con ese nombre");
                }
            }
            else
            {
                throw new DbUpdateException("Asegúrese de insertar un nombre y que sea menor a 35 caracteres");
            }
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

        public bool IsValidName(string name)
        {
            if(name == null)
            {
                return false;
            }
            else
            {
                bool isValid = name.Length > 0 && name.Length < 35;
                return isValid;
            }
        }

        public async Task<bool> IsNotRepeated(string name)
        {
            var existingCityCount = Task.Run(() => _context.Cities.Count(a => a.Name == name));
            int results = await existingCityCount;
            if (results == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [Serializable]
    public class CityAlreadyExistsException : Exception
    {
        public CityAlreadyExistsException() : base() { }
        public CityAlreadyExistsException(string message) : base(message) { }
        public CityAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        protected CityAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

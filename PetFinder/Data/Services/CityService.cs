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
            if (IsValidName(city.Name))
            {
                if (await IsRepeated(city.SerializedName))
                {
                    throw new CityAlreadyExistsException("Ya existe una ciudad con ese nombre");
                }
                if (city.Id > 0)
                {
                    return await Update(city);
                }
                return await Insert(city);
            }
            throw new DbUpdateException("Asegúrese de insertar un nombre y que sea menor a 35 caracteres");
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

        public async Task<bool> IsRepeated(string serializedName)
        {
            var existingCityCount = await Task.Run(() => _context.Cities.Count(c => c.SerializedName == serializedName));
            if (existingCityCount > 0)
            {
                return true;
            }
            return false;
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

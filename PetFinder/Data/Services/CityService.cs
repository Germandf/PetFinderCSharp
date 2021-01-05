using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class CityService : ICityService
    {
        private readonly PetFinderContext _context;
        private readonly ILogger _logger;
        public const string INVALID_NAME_ERROR = "El nombre ingresado no corresponde a un nombre valido";
        public const string REPEATED_CITY_ERROR = "Ya existe la ciudad";
        public const string SAVING_ERROR = "Ocurrio un error al guardar";

        public CityService( PetFinderContext context,
                            ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Delete(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            _context.Cities.Remove(city);
            _logger.Warning("City {name} deleted, Id: {id}", city.Name, city.Id);
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
            _logger.Information("City {name} created", city.Name);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(City city)
        {
            GenericResult result = new GenericResult();
            if (IsValidName(city.Name))
            {
                city.SerializedName = city.Name.ToUpper().Replace(" ", "");
                if (await IsRepeated(city.SerializedName) && city.Id == 0)
                {
                    //Devolver result con mensaje de error
                    result.AddError(REPEATED_CITY_ERROR);
                }
                if (result.Success)
                {
                    if (city.Id > 0 )
                    {
                        if (!await Update(city))
                        {
                            result.AddError(SAVING_ERROR);
                        }
                    }
                    else if (!await Insert(city)) result.AddError(SAVING_ERROR);
                }
            }
            else
            {
                result.AddError(INVALID_NAME_ERROR);
            }
            return result;
        }

        public async Task<bool> Update(City city)
        {
            _context.Entry(city).State = EntityState.Modified;
            _logger.Information("City {name} updated, Id: {id}", city.Name, city.Id);
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

        public async Task<bool> HasPetsAssociated(City city)
        {
            var petsFromThisCity = await _context.Pets.
                Include(p => p.City).
                Where(p => p.City.SerializedName == city.SerializedName).
                ToListAsync();
            if(petsFromThisCity.Count() == 0)
            {
                return false;
            }
            return true;
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

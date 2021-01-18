using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;
using PetFinder.ViewModels;
using Serilog;

namespace PetFinder.Data
{
    public interface ICityService
    {
        /// <summary>
        ///     Gets all Cities
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type City
        /// </returns>
        Task<IEnumerable<City>> GetAll();

        /// <summary>
        ///     Gets a specific City by its Id
        /// </summary>
        /// <returns>
        ///     A City object
        /// </returns>
        Task<City> Get(int id);

        /// <summary>
        ///     Inserts a City
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(City city);

        /// <summary>
        ///     Updates a City
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(City city);

        /// <summary>
        ///     Deletes a City
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Inserts or Updates a City depending on the case.
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(City city);

        /// <summary>
        ///     Transforms the CityViewModel into a City and calls Save with the City parameter
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(CityViewModel cityViewModel);

        /// <summary>
        ///     Checks if the name is used by an already created City
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);

        /// <summary>
        ///     Checks if exists a Pet that depends on a specific City
        /// </summary>
        /// <returns>
        ///     A bool that indicates if there is at least one
        /// </returns>
        Task<bool> HasPetsAssociated(City city);
    }

    public class CityService : ICityService
    {
        public const string INVALID_NAME_ERROR = "El nombre ingresado no corresponde a un nombre valido";
        public const string REPEATED_CITY_ERROR = "Ya existe la ciudad";
        public const string SAVING_ERROR = "Ocurrio un error al guardar";
        private readonly PetFinderContext _context;
        private readonly ILogger _logger;

        public CityService(PetFinderContext context,
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
            var city = await _context.Cities.FindAsync(id);
            _context.Entry(city).State = EntityState.Detached;
            return city;
        }

        public async Task<bool> Insert(City city)
        {
            _context.Cities.Add(city);
            _logger.Information("City {name} created", city.Name);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(CityViewModel cityViewModel)
        {
            var city = cityViewModel.ConvertToCity();
            return await Save(city);
        }

        public async Task<GenericResult> Save(City city)
        {
            var result = new GenericResult();
            if (IsValidName(city.Name))
            {
                city.SerializedName = city.Name.ToUpper().Replace(" ", "");
                if (await IsRepeated(city.SerializedName) && city.Id == 0) result.AddError(REPEATED_CITY_ERROR);
                if (result.Success)
                {
                    if (city.Id > 0)
                    {
                        if (!await Update(city)) result.AddError(SAVING_ERROR);
                    }
                    else if (!await Insert(city))
                    {
                        result.AddError(SAVING_ERROR);
                    }
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

        public async Task<bool> IsRepeated(string serializedName)
        {
            var existingCityCount =
                await Task.Run(() => _context.Cities.Count(c => c.SerializedName == serializedName));
            if (existingCityCount > 0) return true;
            return false;
        }

        public async Task<bool> HasPetsAssociated(City city)
        {
            var petsFromThisCity = await _context.Pets.Include(p => p.City)
                .Where(p => p.City.SerializedName == city.SerializedName).ToListAsync();
            if (petsFromThisCity.Count() == 0) return false;
            return true;
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }

            var isValid = name.Length > 0 && name.Length < 35;
            return isValid;
        }
    }

    [Serializable]
    public class CityAlreadyExistsException : Exception
    {
        public CityAlreadyExistsException()
        {
        }

        public CityAlreadyExistsException(string message) : base(message)
        {
        }

        public CityAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CityAlreadyExistsException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
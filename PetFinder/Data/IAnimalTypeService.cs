using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;
using PetFinder.ViewModels;
using Serilog;

namespace PetFinder.Data
{
    public interface IAnimalTypeService
    {
        /// <summary>
        ///     Gets all AnimalTypes
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type AnimalType
        /// </returns>
        Task<IEnumerable<AnimalType>> GetAll();

        /// <summary>
        ///     Gets a specific AnimalType by its Id
        /// </summary>
        /// <returns>
        ///     An AnimalType object
        /// </returns>
        Task<AnimalType> Get(int id);

        /// <summary>
        ///     Inserts an AnimalType
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(AnimalType animalType);

        /// <summary>
        ///     Updates an AnimalType
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(AnimalType animalType);

        /// <summary>
        ///     Deletes an AnimalType
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Inserts or Updates an AnimalType depending on the case.
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(AnimalType animalType);

        /// <summary>
        ///     Transforms the AnimalTypeViewModel into an AnimalType and calls Save with the AnimalType parameter
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(AnimalTypeViewModel animalTypeViewModel);

        /// <summary>
        ///     Checks if the name is used by an already created AnimalType
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);

        /// <summary>
        ///     Checks if exists a Pet that depends on a specific AnimalType
        /// </summary>
        /// <returns>
        ///     A bool that indicates if there is at least one
        /// </returns>
        Task<bool> HasPetsAssociated(AnimalType animalType);
    }

    public class AnimalTypeService : IAnimalTypeService
    {
        public const string INVALID_NAME_ERROR = "El nombre ingresado no corresponde a un nombre valido";
        public const string REPEATED_ANIMAL_TYPE_ERROR = "Ya existe el tipo de animal";
        public const string SAVING_ERROR = "Ocurrio un error al guardar";
        private readonly PetFinderContext _context;
        private readonly ILogger _logger;

        public AnimalTypeService(PetFinderContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Delete(int id)
        {
            var animalType = await Get(id);
            _context.Remove(animalType);
            _logger.Warning("AnimalType {name} deleted, Id: {id}", animalType.Name, animalType.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AnimalType>> GetAll()
        {
            return await _context.AnimalTypes.ToListAsync();
        }

        public async Task<AnimalType> Get(int id)
        {
            var animalType = await _context.AnimalTypes.FindAsync(id);
            _context.Entry(animalType).State = EntityState.Detached;
            return animalType;
        }

        public async Task<bool> Insert(AnimalType animalType)
        {
            _context.AnimalTypes.Add(animalType);
            _logger.Information("AnimalType {name} created", animalType.Name);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(AnimalType animalType)
        {
            //Le decimos a la DB que el animalType fue modificado
            _context.Entry(animalType).State = EntityState.Modified;
            _logger.Information("AnimalType {name} updated, Id: {id}", animalType.Name, animalType.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GenericResult> Save(AnimalTypeViewModel animalTypeViewModel)
        {
            var animalType = animalTypeViewModel.ConvertToAnimalType();
            return await Save(animalType);
        }

        public async Task<GenericResult> Save(AnimalType animalType)
        {
            var result = new GenericResult();
            if (IsValidName(animalType.Name))
            {
                animalType.SerializedName = animalType.Name.ToUpper().Replace(" ", "");
                if (await IsRepeated(animalType.SerializedName) && animalType.Id == 0)
                    result.AddError(REPEATED_ANIMAL_TYPE_ERROR);
                if (result.Success)
                {
                    if (animalType.Id > 0)
                    {
                        if (!await Update(animalType)) result.AddError(SAVING_ERROR);
                    }
                    else if (!await Insert(animalType))
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

        public async Task<bool> IsRepeated(string serializedName)
        {
            var existingAnimalTypeCount =
                await Task.Run(() => _context.AnimalTypes.Count(a => a.SerializedName == serializedName));
            if (existingAnimalTypeCount > 0) return true;
            return false;
        }

        public async Task<bool> HasPetsAssociated(AnimalType animalType)
        {
            var petsFromThisAnimalType = await _context.Pets.Include(p => p.AnimalType)
                .Where(p => p.AnimalType.SerializedName == animalType.SerializedName).ToListAsync();
            if (petsFromThisAnimalType.Count() == 0) return false;
            return true;
        }

        public bool IsValidName(string name)
        {
            if (name == null) return false;
            if (name.Length <= 0 || name.Length > 35) return false;
            // Chequeo que sean caracteres de a - Z con espacios
            var match = Regex.Match(name, "^[a-zA-Z ]+$");
            if (!match.Success) return false;
            return true;
        }
    }

    [Serializable]
    public class AnimalTypeAlreadyExistsException : Exception
    {
        public AnimalTypeAlreadyExistsException()
        {
        }

        public AnimalTypeAlreadyExistsException(string message) : base(message)
        {
        }

        public AnimalTypeAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AnimalTypeAlreadyExistsException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
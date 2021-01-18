using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetFinder.Helpers;
using PetFinder.Models;

namespace PetFinder.Data
{
    public interface IGenderService
    {
        /// <summary>
        ///     Gets all Genders
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type Gender
        /// </returns>
        Task<IEnumerable<Gender>> GetAll();

        /// <summary>
        ///     Gets a specific Gender by its Id
        /// </summary>
        /// <returns>
        ///     A Gender object
        /// </returns>
        Task<Gender> Get(int id);

        /// <summary>
        ///     Inserts a Gender
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(Gender gender);

        /// <summary>
        ///     Updates a Gender
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(Gender gender);

        /// <summary>
        ///     Deletes a Gender
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Inserts or Updates a Gender depending on the case.
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<GenericResult> Save(Gender gender);

        /// <summary>
        ///     Checks if the name is used by an already created Gender
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);
    }

    public class GenderService : IGenderService
    {
        private readonly PetFinderContext _context;

        public GenderService(PetFinderContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int id)
        {
            var gender = await _context.Genders.FindAsync(id);
            _context.Genders.Remove(gender);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Gender> Get(int id)
        {
            return await _context.Genders.FindAsync(id);
        }

        public async Task<IEnumerable<Gender>> GetAll()
        {
            return await _context.Genders.ToListAsync();
        }

        public async Task<bool> Insert(Gender gender)
        {
            _context.Genders.Add(gender);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsRepeated(string name)
        {
            var existingGenderCount = Task.Run(() => _context.Genders.Count(a => a.Name == name));
            var results = await existingGenderCount;
            if (results == 0)
                return false;
            return true;
        }

        public async Task<GenericResult> Save(Gender gender)
        {
            var genericResult = new GenericResult();
            if (!IsValidName(gender.Name))
            {
                genericResult.Errors.Add("Empty name");
                return genericResult;
            }

            if (await IsRepeated(gender.Name))
            {
                genericResult.Errors.Add("Empty name");
                return genericResult;
            }

            if (gender.Id > 0)
            {
                if (!await Update(gender)) genericResult.Errors.Add("Error on update");
                return genericResult;
            }

            if (!await Insert(gender)) genericResult.Errors.Add("Error on insert");
            return genericResult;
        }

        public async Task<bool> Update(Gender gender)
        {
            _context.Entry(gender).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }

            var isValid = name.Length > 0 && name.Length < 20;
            return isValid;
        }
    }

    [Serializable]
    public class GenderAlreadyExistsException : Exception
    {
        public GenderAlreadyExistsException()
        {
        }

        public GenderAlreadyExistsException(string message) : base(message)
        {
        }

        public GenderAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GenderAlreadyExistsException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
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
            int results = await existingGenderCount;
            if (results == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }
            else
            {
                bool isValid = name.Length > 0 && name.Length < 20;
                return isValid;
            }
        }

        public async Task<bool> Save(Gender gender)
        {
            if (IsValidName(gender.Name))
            {
                if (await IsRepeated(gender.Name))
                {
                    throw new GenderAlreadyExistsException("Ya existe un genero con ese nombre");
                }
                if (gender.Id > 0)
                {
                    return await Update(gender);
                }
                return await Insert(gender);
            }
            throw new DbUpdateException("Asegúrese de insertar un nombre y que sea menor a 20 caracteres");
        }

        public async Task<bool> Update(Gender gender)
        {
            _context.Entry(gender).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }

    [Serializable]
    public class GenderAlreadyExistsException : Exception
    {
        public GenderAlreadyExistsException() : base() { }
        public GenderAlreadyExistsException(string message) : base(message) { }
        public GenderAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        protected GenderAlreadyExistsException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

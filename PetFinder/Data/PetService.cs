using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class PetService : IPetService
    {
        private readonly PetFinderContext _context;

        public PetService(PetFinderContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            _context.Pets.Remove(pet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Pet> Get(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetAll()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<bool> Insert(Pet pet)
        {
            _context.Pets.Add(pet);
            return await _context.SaveChangesAsync() > 0;
        }

        public bool IsValidName(string name)
        {
            if (name == null)
            {
                return false;
            }
            else
            {
                bool isValid = name.Length > 0 && name.Length <= 20;
                return isValid;
            }
        }

        public async Task<bool> Save(Pet pet)
        {
            if (IsValidName(pet.Name))
            {
                if (pet.Id > 0)
                {
                    return await Update(pet);
                }
                return await Insert(pet);
            }
            throw new DbUpdateException("Asegúrese de insertar un nombre y que sea menor a 20 caracteres");
        }

        public async Task<bool> Update(Pet pet)
        {
            _context.Entry(pet).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

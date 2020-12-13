using Microsoft.EntityFrameworkCore;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class AnimalTypeService : IAnimalTypeService
    {
        private readonly PetFinderContext _context;

        public AnimalTypeService(PetFinderContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(int id)
        {
            var animalTypeToDelte = await Get(id);
            _context.Remove(animalTypeToDelte);

            //Nos devuelve cuantas lineas elimino si elimino más que 0 quiere decir que elimino ok
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<IEnumerable<AnimalType>> GetAll()
        {
            return await _context.AnimalTypes.ToListAsync();
        }

        public async Task<AnimalType> Get(int Id)
        {
            return await _context.AnimalTypes.FindAsync(Id);
        }

        public async Task<bool> Insert(AnimalType animalType)
        {
            _context.AnimalTypes.Add(animalType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(AnimalType animalType)
        {
            //Le decimos a la DB que el animalType fue modificado
            _context.Entry(animalType).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Save(AnimalType animalType)
        {
            if(animalType.Id > 0)
            {
                //El tipo de animal existe por lo tanto debemos hacer un update 
                return await Update(animalType);
            }
            return await Insert(animalType);
        }
    }
}

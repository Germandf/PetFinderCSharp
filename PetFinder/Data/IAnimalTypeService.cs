using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IAnimalTypeService
    {
        Task<IEnumerable<Models.AnimalType>> GetAll();
        Task<AnimalType> GetAnimalType(int Id);
        Task<bool> InsertAnimalType(AnimalType animalType);
        Task<bool> UpdateAnimalType(AnimalType animalType);
        Task<bool> DeleteAnimalType(int id );
        Task<bool> Save(AnimalType animalType);


    }
}

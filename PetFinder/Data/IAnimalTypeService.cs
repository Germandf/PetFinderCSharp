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
        Task<AnimalType> Get(int Id);
        Task<bool> Insert(AnimalType animalType);
        Task<bool> Update(AnimalType animalType);
        Task<bool> Delete(int id );
        Task<bool> Save(AnimalType animalType);
        Task<bool> IsRepeated(string name);


    }
}

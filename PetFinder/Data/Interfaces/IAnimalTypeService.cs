using PetFinder.Helpers;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IAnimalTypeService
    {
        Task<IEnumerable<AnimalType>> GetAll();
        Task<AnimalType> Get(int Id);
        Task<bool> Insert(AnimalType animalType);
        Task<bool> Update(AnimalType animalType);
        Task<bool> Delete(int id );
        Task<GenericResult> Save(AnimalType animalType);
        Task<bool> IsRepeated(string name);
        Task<bool> HasNoPetsAssociated(AnimalType animalType);
    }
}

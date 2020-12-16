using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IPetService
    {
        Task<IEnumerable<Pet>> GetAll();
        Task<Pet> Get(int id);
        Task<bool> Insert(Pet pet);
        Task<bool> Update(Pet pet);
        Task<bool> Delete(int id);
        Task<bool> Save(Pet pet);
        bool IsValidName(string name);
    }
}

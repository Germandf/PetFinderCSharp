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
        Task<bool> Update(Pet pet, List<string> errorMessages);
        Task<bool> Delete(int id);
        Task<bool> Save(Pet pet, List<String> errorMessages);
        Task<IEnumerable<Pet>> GetAllByUser(string UserId);
        Task<bool> currUserCanEdit(Pet pet);
        Task<bool> SetFound(int id);
        bool IsValidName(string name);
    }
}

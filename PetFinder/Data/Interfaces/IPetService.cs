using BlazorInputFile;
using PetFinder.Areas.Identity;
using PetFinder.Helpers;
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
        Task<GenericResult> Update(Pet pet);
        Task<bool> Delete(int id);
        Task<GenericResult> Save(Pet pet, IFileListEntry photo);
        Task<IEnumerable<Pet>> GetAllByUser(string UserId);
        Task<IEnumerable<Pet>> GetAllByFilter(params string[] filters);
        Task<bool> CurrUserCanEdit(Pet pet);
        Task<bool> SetFound(int id);
        bool IsValidName(string name);
    }
}

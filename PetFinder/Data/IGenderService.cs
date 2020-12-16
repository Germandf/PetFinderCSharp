using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IGenderService
    {
        Task<IEnumerable<Gender>> GetAll();
        Task<Gender> Get(int id);
        Task<bool> Insert(Gender gender);
        Task<bool> Update(Gender gender);
        Task<bool> Delete(int id);
        Task<bool> Save(Gender gender);
        bool IsValidName(string name);
        Task<bool> IsNotRepeated(string name);
    }
}

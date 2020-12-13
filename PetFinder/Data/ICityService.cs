using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAll();
        Task<City> Get(int id);
        Task<bool> Insert(City city);
        Task<bool> Update(City city);
        Task<bool> Delete(int id);
        Task<bool> Save(City city);
    }
}

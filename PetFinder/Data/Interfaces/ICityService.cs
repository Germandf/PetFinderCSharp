﻿using PetFinder.Helpers;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface ICityService
    {
        Task<IEnumerable<City>> GetAll();
        Task<City> Get(int id);
        Task<bool> Insert(City city);
        Task<bool> Update(City city);
        Task<bool> Delete(int id);
        Task<GenericResult> Save(City city);
        bool IsValidName(string name);
        Task<bool> IsRepeated(string name);
        Task<bool> HasPetsAssociated(City city);
    }
}

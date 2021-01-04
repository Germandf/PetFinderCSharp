using PetFinder.Helpers;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface ICityService
    {
        /// <summary>
        /// Gets all Cities
        /// </summary>
        /// <returns>
        /// An IEnumerable of type City
        /// </returns>
        Task<IEnumerable<City>> GetAll();

        /// <summary>
        /// Gets a specific City by its Id
        /// </summary>
        /// <returns>
        /// A City object
        /// </returns>
        Task<City> Get(int id);

        /// <summary>
        /// Inserts a City
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(City city);

        /// <summary>
        /// Updates a City
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(City city);

        /// <summary>
        /// Deletes a City
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Inserts or Updates a City depending on the case.
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it was successfull or not
        /// </returns>
        Task<GenericResult> Save(City city);

        /// <summary>
        /// Checks if the name is used by an already created City
        /// </summary>
        /// <returns>
        /// A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);

        /// <summary>
        /// Checks if exists a Pet that depends on a specific City
        /// </summary>
        /// <returns>
        /// A bool that indicates if there is at least one
        /// </returns>
        Task<bool> HasPetsAssociated(City city);
    }
}

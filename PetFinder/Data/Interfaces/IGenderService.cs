using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    interface IGenderService
    {
        /// <summary>
        /// Gets all Genders
        /// </summary>
        /// <returns>
        /// An IEnumerable of type Gender
        /// </returns>
        Task<IEnumerable<Gender>> GetAll();

        /// <summary>
        /// Gets a specific Gender by its Id
        /// </summary>
        /// <returns>
        /// A Gender object
        /// </returns>
        Task<Gender> Get(int id);

        /// <summary>
        /// Inserts a Gender
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(Gender gender);

        /// <summary>
        /// Updates a Gender
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(Gender gender);

        /// <summary>
        /// Deletes a Gender
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Inserts or Updates a Gender depending on the case.
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Save(Gender gender);

        /// <summary>
        /// Checks if the name is used by an already created Gender
        /// </summary>
        /// <returns>
        /// A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);
    }
}

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
        /// <summary>
        /// Gets all AnimalTypes
        /// </summary>
        /// <returns>
        /// An IEnumerable of type AnimalType
        /// </returns>
        Task<IEnumerable<AnimalType>> GetAll();

        /// <summary>
        /// Gets a specific AnimalType by its Id
        /// </summary>
        /// <returns>
        /// An AnimalType object
        /// </returns>
        Task<AnimalType> Get(int Id);

        /// <summary>
        /// Inserts an AnimalType
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(AnimalType animalType);

        /// <summary>
        /// Updates an AnimalType
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Update(AnimalType animalType);

        /// <summary>
        /// Deletes an AnimalType
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Insert or Update an AnimalType depending on the case.
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it was successfull or not
        /// </returns>
        Task<GenericResult> Save(AnimalType animalType);

        /// <summary>
        /// Check if a name is used by an already created AnimalType
        /// </summary>
        /// <returns>
        /// A bool that indicates if it's being used or not
        /// </returns>
        Task<bool> IsRepeated(string name);

        /// <summary>
        /// Check if exists a Pet that depends on a specific AnimalType
        /// </summary>
        /// <returns>
        /// A bool that indicates if there is at least one
        /// </returns>
        Task<bool> HasNoPetsAssociated(AnimalType animalType);
    }
}

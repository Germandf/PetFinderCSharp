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
        /// <summary>
        /// Gets all Pets with their referenced City, AnimalType and Gender
        /// </summary>
        /// <returns>
        /// An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAll();

        /// <summary>
        /// Gets a specific Pet with its referenced City, AnimalType and Gender by its Id
        /// </summary>
        /// <returns>
        /// A Pet object that includes City, AnimalType and Gender
        /// </returns>
        Task<Pet> Get(int id);

        /// <summary>
        /// Inserts a Pet
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Insert(Pet pet);

        /// <summary>
        /// Updates a Pet
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<GenericResult> Update(Pet pet);

        /// <summary>
        /// Deletes a Pet
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Inserts or Updates a Pet depending on the case, it also takes care of uploading the photo if necessary
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Save(Pet pet, IFileListEntry photo);

        /// <summary>
        /// Looks for all Pets that contains the same UserId attribute as the one inserted
        /// </summary>
        /// <returns>
        /// An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAllByUser(string UserId);

        /// <summary>
        /// Looks for all Pets that matches with the filters inserted, these can be single or multiple
        /// </summary>
        /// <returns>
        /// An IEnumerable of type Pet that each one includes City, AnimalType and Gender
        /// </returns>
        Task<IEnumerable<Pet>> GetAllByFilter(params string[] filters);

        /// <summary>
        /// Checks if the current user in session has permissions on this Pet
        /// </summary>
        /// <returns>
        /// A bool that indicates if he has permissions or not
        /// </returns>
        Task<bool> CurrUserCanEdit(Pet pet);

        /// <summary>
        /// Sets the Found attribute of the Pet object as true
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> SetFound(int id);
    }
}

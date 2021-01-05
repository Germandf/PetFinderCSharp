using PetFinder.Areas.Identity;
using PetFinder.Helpers;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinderApi.Data.Interfaces
{
    public interface ICommentService
    {
        /// <summary>
        /// Gets a specific Comment by its Id with its referenced Application User inside
        /// </summary>
        /// <returns>
        /// A Comment object
        /// </returns>
        Task<Comment> Get(int id);

        /// <summary>
        /// Inserts a Comment
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Insert(Comment comment);

        /// <summary>
        /// Updates a Comment
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it was successfull or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Update(Comment comment);

        /// <summary>
        /// Deletes a Comment
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Gets all Comments with their referenced Application User inside from one Pet by the Pet's Id
        /// </summary>
        /// <returns>
        /// An IEnumerable of type Comment
        /// </returns>
        Task<IEnumerable<Comment>> GetAllFromPet(int id);

        /// <summary>
        /// Checks if a Comment exists by its Id
        /// </summary>
        /// <returns>
        /// A bool that indicates if it exists or not
        /// </returns>
        Task<bool> Exists(int id);

        /// <summary>
        /// Checks if the current User in session has permissions on this Comment
        /// </summary>
        /// <returns>
        /// A bool that indicates if he has permissions or not
        /// </returns>
        Task<bool> UserCanEdit(string userEmail, int commentId);

        /// <summary>
        /// Checks if the Comment has correct PetId and UserId
        /// </summary>
        /// <returns>
        /// A GenericResult that indicates if it has correct data or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> HasCorrectData(Comment comment);
    }
}

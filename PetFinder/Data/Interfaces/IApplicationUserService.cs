using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public interface IApplicationUserService
    {
        /// <summary>
        /// Gets all Users
        /// </summary>
        /// <returns>
        /// An IEnumerable of type ApplicationUser
        /// </returns>
        Task<IEnumerable<ApplicationUser>> GetAll();

        /// <summary>
        /// Changes the User's role from Admin to User
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Downgrade(string userId);

        /// <summary>
        /// Changes the User's role from Admin to User
        /// </summary>
        /// <returns>
        /// A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Upgrade(string userId);

        /// <summary>
        /// Gets the current User in session
        /// </summary>
        /// <returns>
        /// An ApplicationUser object
        /// </returns>
        Task<ApplicationUser> GetCurrent();

        /// <summary>
        /// Gets a specific User by its Id
        /// </summary>
        /// <returns>
        /// An ApplicationUser object
        /// </returns>
        Task<ApplicationUser> GetById(string Id);

        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <returns>
        /// An IdentityResult operation that indicates if it succeeded or not
        /// </returns>
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
    }
}

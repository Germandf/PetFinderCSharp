using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Models;
using Serilog;

namespace PetFinder.Data
{
    public interface IApplicationUserService
    {
        /// <summary>
        ///     Gets all Users
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type ApplicationUser
        /// </returns>
        Task<IEnumerable<ApplicationUser>> GetAll();

        /// <summary>
        ///     Changes the User's role from Admin to User
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Downgrade(string userId);

        /// <summary>
        ///     Changes the User's role from Admin to User
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successfull or not
        /// </returns>
        Task<bool> Upgrade(string userId);

        /// <summary>
        ///     Gets the current User in session
        /// </summary>
        /// <returns>
        ///     An ApplicationUser object
        /// </returns>
        Task<ApplicationUser> GetCurrent();

        /// <summary>
        ///     Gets a specific User by its Id
        /// </summary>
        /// <returns>
        ///     An ApplicationUser object
        /// </returns>
        Task<ApplicationUser> GetById(string Id);

        /// <summary>
        ///     Deletes a User
        /// </summary>
        /// <returns>
        ///     An IdentityResult operation that indicates if it succeeded or not
        /// </returns>
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
    }

    public class ApplicationUserService : IApplicationUserService
    {
        public static string ROLE_ADMIN = "Administrator";
        public static string ROLE_USER = "Normal";

        private readonly PetFinderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _loger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(PetFinderContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger loger)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _loger = loger;
        }

        public async Task<bool> Downgrade(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var resultRemoveRole = await _userManager.AddToRoleAsync(user, "Normal");
            var currUser = await GetCurrent();
            if (resultRemoveRole.Succeeded)
            {
                var resultAdd = await _userManager.RemoveFromRoleAsync(user, "Administrator");

                if (resultAdd.Succeeded)
                {
                    _loger.Information("{user} role changed to NORMAL USER by {currUser}", user.Email, currUser.Email);
                    return true;
                }
            }

            _loger.Warning("{currUser} try to change role to NORMAL USER of {user} without success", currUser.Email,
                user.Email);
            return false;
        }

        public async Task<bool> Upgrade(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var resultAdd = await _userManager.RemoveFromRoleAsync(user, "Normal");
            var currUser = await GetCurrent();
            if (resultAdd.Succeeded)
            {
                var resultRemoveRole = await _userManager.AddToRoleAsync(user, "Administrator");
                if (resultRemoveRole.Succeeded)
                {
                    _loger.Information("{user} role changed to ADMIN by {currUser}", user.Email, currUser.Email);

                    return true;
                }
            }

            _loger.Warning("{currUser} try to change role to ADMIN of {user} without success", currUser.Email,
                user.Email);
            return false;
        }

        public async Task<ApplicationUser> GetCurrent()
        {
            var currUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currUser == null) return null;
            if (await _userManager.IsInRoleAsync(currUser, ROLE_ADMIN)) currUser.SetAdmin(true);
            return currUser;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(user.Id));
            var currUser = await GetCurrent();
            if (result.Succeeded)
                _loger.Warning("{user} deleted by {currUser} successfully", user.Email, currUser.Email);
            else
                _loger.Warning("{currUser} try delete  {user} without success", currUser.Email, user.Email);
            return result;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            IEnumerable<ApplicationUser> users = await _context.AspNetUsers.ToListAsync();
            foreach (var user in users)
                if (await _userManager.IsInRoleAsync(user, ROLE_ADMIN))
                    user.SetAdmin(true);
                else
                    user.SetAdmin(false);
            return users;
        }

        public async Task<ApplicationUser> GetById(string Id)
        {
            return await _context.AspNetUsers.FindAsync(Id);
        }
    }
}
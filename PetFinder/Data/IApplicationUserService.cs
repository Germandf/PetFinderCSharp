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
        ///     A bool that indicates if it was successful or not
        /// </returns>
        Task<bool> Downgrade(string userId);

        /// <summary>
        ///     Changes the User's role from Admin to User
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successful or not
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
        public const string RoleAdmin = "Administrator";
        public const string RoleUser = "Normal";

        private readonly PetFinderContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(PetFinderContext context,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger logger)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> Downgrade(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var resultRemoveRole = await _userManager.AddToRoleAsync(user, "Normal");
            var currentUser = await GetCurrent();
            if (resultRemoveRole.Succeeded)
            {
                var resultAdd = await _userManager.RemoveFromRoleAsync(user, "Administrator");

                if (resultAdd.Succeeded)
                {
                    _logger.Information("{user} role changed to NORMAL USER by {currentUser}", user.Email, currentUser.Email);
                    return true;
                }
            }

            _logger.Warning("{currentUser} try to change role to NORMAL USER of {user} without success", currentUser.Email,
                user.Email);
            return false;
        }

        public async Task<bool> Upgrade(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var resultAdd = await _userManager.RemoveFromRoleAsync(user, "Normal");
            var currentUser = await GetCurrent();
            if (resultAdd.Succeeded)
            {
                var resultRemoveRole = await _userManager.AddToRoleAsync(user, "Administrator");
                if (resultRemoveRole.Succeeded)
                {
                    _logger.Information("{user} role changed to ADMIN by {currentUser}", user.Email, currentUser.Email);

                    return true;
                }
            }

            _logger.Warning("{currentUser} try to change role to ADMIN of {user} without success", currentUser.Email,
                user.Email);
            return false;
        }

        public async Task<ApplicationUser> GetCurrent()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser == null) return null;
            if (await _userManager.IsInRoleAsync(currentUser, RoleAdmin)) currentUser.SetAdmin(true);
            return currentUser;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(user.Id));
            var currentUser = await GetCurrent();
            if (result.Succeeded)
                _logger.Warning("{user} deleted by {currentUser} successfully", user.Email, currentUser.Email);
            else
                _logger.Warning("{currentUser} try delete  {user} without success", currentUser.Email, user.Email);
            return result;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            IEnumerable<ApplicationUser> users = await _context.AspNetUsers.ToListAsync();
            foreach (var user in users)
                if (await _userManager.IsInRoleAsync(user, RoleAdmin))
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
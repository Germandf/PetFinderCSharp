using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Data;
using PetFinder.Helpers;
using PetFinder.Models;
using Serilog;

namespace PetFinderApi.Data
{
    public interface ICommentService
    {
        /// <summary>
        ///     Gets a specific Comment by its Id with its referenced Application User inside
        /// </summary>
        /// <returns>
        ///     A Comment object
        /// </returns>
        Task<Comment> Get(int id);

        /// <summary>
        ///     Inserts a Comment
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successful or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Insert(Comment comment);

        /// <summary>
        ///     Updates a Comment
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it was successful or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> Update(Comment comment);

        /// <summary>
        ///     Deletes a Comment
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it was successful or not
        /// </returns>
        Task<bool> Delete(int id);

        /// <summary>
        ///     Gets all Comments with their referenced Application User inside from one Pet by the Pet's Id
        /// </summary>
        /// <returns>
        ///     An IEnumerable of type Comment
        /// </returns>
        Task<IEnumerable<Comment>> GetAllFromPet(int id);

        /// <summary>
        ///     Checks if a Comment exists by its Id
        /// </summary>
        /// <returns>
        ///     A bool that indicates if it exists or not
        /// </returns>
        Task<bool> Exists(int id);

        /// <summary>
        ///     Checks if a User has permissions on a Comment
        /// </summary>
        /// <returns>
        ///     A bool that indicates if he has permissions or not
        /// </returns>
        Task<bool> UserCanEdit(string userEmail, int commentId);

        /// <summary>
        ///     Checks if the Comment has correct PetId and UserId
        /// </summary>
        /// <returns>
        ///     A GenericResult that indicates if it has correct data or not, if not, it will contain the error/s
        /// </returns>
        Task<GenericResult> HasCorrectData(Comment comment);
    }

    public class CommentService : ICommentService
    {
        public CommentService(PetFinderContext context,
            ILogger logger,
            UserManager<ApplicationUser> userManager,
            PetService petService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _petService = petService;
        }

        public async Task<bool> UserCanEdit(string userEmail, int commentId)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return false;
            var isAdmin = await _userManager.IsInRoleAsync(user, ApplicationUserService.RoleAdmin);
            var comment = await Get(commentId);
            if (comment == null)
                //Si no existe devuelvo true asi no da error de login, luego HasCorrectData detecta que no existe
                return true;
            return comment.UserId == user.Id || isAdmin;
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            _logger.Warning("Comment {message} deleted, Id: {id}", comment.Message, comment.Id);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task<Comment> Get(int id)
        {
            var comment = await _context.Comments.Select(x =>
                new Comment
                {
                    PetId = x.PetId,
                    Id = x.Id,
                    Rate = x.Rate,
                    Message = x.Message,
                    UserId = x.UserId,
                    User = new ApplicationUser {Name = x.User.Name, Surname = x.User.Surname}
                }
            ).Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Entry(comment).State = EntityState.Detached;
            return comment;
        }

        public async Task<IEnumerable<Comment>> GetAllFromPet(int id)
        {
            return await _context.Comments.Where(c => c.PetId == id).Select(x =>
                new Comment
                {
                    PetId = x.PetId,
                    Id = x.Id,
                    Rate = x.Rate,
                    Message = x.Message,
                    UserId = x.UserId,
                    User = new ApplicationUser {Name = x.User.Name, Surname = x.User.Surname}
                }
            ).ToListAsync();
        }

        public async Task<GenericResult> Insert(Comment comment)
        {
            var hasCorrectData = await HasCorrectData(comment);
            if (hasCorrectData.Success)
            {
                _context.Comments.Add(comment);
                if (await _context.SaveChangesAsync() == 0) hasCorrectData.AddError(ERROR_SAVING_COMMENT);
            }

            return hasCorrectData;
        }

        public async Task<GenericResult> Update(Comment comment)
        {
            var hasCorrectData = await HasCorrectData(comment);
            if (hasCorrectData.Success)
            {
                var commentExists = await Exists(comment.Id);
                if (commentExists)
                {
                    _context.Entry(comment).State = EntityState.Modified;
                    if (await _context.SaveChangesAsync() == 0) hasCorrectData.AddError(ERROR_SAVING_COMMENT);
                }
                else
                {
                    hasCorrectData.AddError(ERROR_COMMENT_NOT_FOUND);
                }
            }

            return hasCorrectData;
        }

        public async Task<GenericResult> HasCorrectData(Comment comment)
        {
            var result = new GenericResult();
            if (await _petService.Get(comment.PetId) == null) result.AddError(ERROR_WRONG_PET);
            if (await _userManager.FindByIdAsync(comment.UserId) == null) result.AddError(ERROR_WRONG_USER);
            return result;
        }

        #region

        public static string ERROR_WRONG_PET = "La mascota no existe";
        public static string ERROR_WRONG_USER = "El usuario no existe";
        public static string ERROR_SAVING_COMMENT = "La operación finalizó con un error, intente más tarde";
        public static string ERROR_COMMENT_NOT_FOUND = "El comentario no existe";

        private readonly PetFinderContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PetService _petService;

        #endregion
    }
}
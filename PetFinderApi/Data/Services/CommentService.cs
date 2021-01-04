using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Data;
using PetFinder.Helpers;
using PetFinder.Models;
using PetFinderApi.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinderApi.Data.Services
{
    public class CommentService : ICommentService
    {
        #region
        public static string ERROR_WRONG_PET = "La mascota no existe";
        public static string ERROR_WRONG_USER = "El usuario no existe";
        public static string ERROR_SAVING_COMMENT = "La operación finalizó con un error, intente más tarde";
        public static string ERROR_COMMENT_NOT_FOUND = "El comentario no existe";

        private readonly PetFinderContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PetService _petService;
        #endregion

        public CommentService(  PetFinderContext context, 
                                UserManager<ApplicationUser> userManager,
                                PetService petService)
        {
            _context = context;
            _userManager = userManager;
            _petService = petService;
        }

        public async Task<bool> UserCanEdit(string userEmail, int commentId)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
            if(user == null)
            {
                return false;
            }
            bool isAdmin = await _userManager.IsInRoleAsync(user, ApplicationUserService.ROLE_ADMIN);
            Comment comment = await Get(commentId);
            _context.Entry(comment).State = EntityState.Detached;
            if(comment == null)
            {
                //Si no existe devuelvo true asi no da error de login, luego HasCorrectData detecta que no existe
                return true; 
            }
            return (comment.UserId == user.Id || isAdmin);
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task<Comment> Get(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetAllFromPet(int id)
        {
            return await _context.Comments.
                Where(c => c.PetId == id).
                ToListAsync();
        }

        public async Task<GenericResult> Insert(Comment comment)
        {
            GenericResult hasCorrectData = await HasCorrectData(comment);
            if (hasCorrectData.Success)
            {
                _context.Comments.Add(comment);
                if(await _context.SaveChangesAsync() == 0)
                {
                    hasCorrectData.AddError(ERROR_SAVING_COMMENT);
                }
            }
            return hasCorrectData;
        }

        public async Task<GenericResult> Update(Comment comment)
        {
            GenericResult hasCorrectData = await HasCorrectData(comment);
            if (hasCorrectData.Success)
            {
                bool commentExists = await Exists(comment.Id);
                if (commentExists)
                {
                    _context.Entry(comment).State = EntityState.Modified;
                    if(await _context.SaveChangesAsync() == 0)
                    {
                        hasCorrectData.AddError(ERROR_SAVING_COMMENT);
                    }
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
            GenericResult result = new GenericResult();
            if (await _petService.Get(comment.PetId) == null)
            {
                result.AddError(ERROR_WRONG_PET);
            }
            if(await _userManager.FindByIdAsync(comment.UserId) == null)
            {
                result.AddError(ERROR_WRONG_USER);
            }
            return result;
        }
    }
}

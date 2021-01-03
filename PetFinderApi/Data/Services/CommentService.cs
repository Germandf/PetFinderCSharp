using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetFinder.Areas.Identity;
using PetFinder.Data;
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
        private readonly PetFinderContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        public CommentService(PetFinderContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public async Task<bool> Insert(Comment comment)
        {
            _context.Comments.Add(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

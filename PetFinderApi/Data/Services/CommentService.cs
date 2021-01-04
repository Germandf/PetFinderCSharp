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
        public async Task<bool> userCanEdit(string userEmail, int commentId)
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
            return await _context.Comments.
                 Select(x =>
                    new Comment
                    {
                        PetId = x.PetId,
                        Id = x.Id,
                        Rate = x.Rate,
                        Message = x.Message,
                        UserId = x.UserId,
                        User = new ApplicationUser { Name = x.User.Name, Surname = x.User.Surname }
                    }
                ).
                Where(x => x.Id == id).
                FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllFromPet(int id)
        {

            return await _context.Comments.
                Where(c => c.PetId == id).
                Select(x => 
                    new Comment
                    {
                        PetId = x.PetId,
                        Id = x.Id,
                        Rate = x.Rate,
                        Message = x.Message,
                        UserId = x.UserId,
                        User = new ApplicationUser { Name = x.User.Name, Surname = x.User.Surname }
                    }
                ).
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

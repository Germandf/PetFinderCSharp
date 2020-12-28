using Microsoft.EntityFrameworkCore;
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
        private readonly PetFinderContext _context;

        public CommentService(PetFinderContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
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

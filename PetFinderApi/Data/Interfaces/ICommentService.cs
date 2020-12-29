using PetFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinderApi.Data.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> Get(int id);
        Task<bool> Insert(Comment comment);
        Task<bool> Update(Comment comment);
        Task<bool> Delete(int id);
        Task<IEnumerable<Comment>> GetAllFromPet(int id);
    }
}

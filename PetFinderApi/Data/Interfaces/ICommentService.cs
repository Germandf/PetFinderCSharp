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
        Task<Comment> Get(int id);
        Task<GenericResult> Insert(Comment comment);
        Task<GenericResult> Update(Comment comment);
        Task<bool> Delete(int id);
        Task<IEnumerable<Comment>> GetAllFromPet(int id);
        Task<bool> Exists(int id);
        Task<bool> UserCanEdit(string userEmail, int commentId);
        Task<GenericResult> HasCorrectData(Comment comment);
    }
}

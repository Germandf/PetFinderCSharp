using PetFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public interface ICommentApiService
    {
        public Task<HttpResponseMessage> CreateComment(CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> DeleteComment(CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> GetComments();

        public Task<HttpResponseMessage> UpdateComment(CommentViewModel commentViewModel);
    }
}

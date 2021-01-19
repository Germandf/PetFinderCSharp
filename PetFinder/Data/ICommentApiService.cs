using PetFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PetFinder.Data
{
    public interface ICommentApiService
    {
        public Task<HttpResponseMessage> CreateComment(CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> DeleteComment(CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> GetComments(int petId);

        public Task<HttpResponseMessage> UpdateComment(CommentViewModel commentViewModel);
    }

    public class CommentApiService : ICommentApiService
    {
        private readonly string _urlApiComments;
        private readonly HttpClient _httpClient = new();

        public CommentApiService(IConfiguration configuration)
        {
            _urlApiComments = configuration["UrlApiController"] + "comentarios";
        }

        public async Task<HttpResponseMessage> CreateComment(CommentViewModel commentViewModel)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> DeleteComment(CommentViewModel commentViewModel)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> GetComments(int petId)
        {
            // GET api/comentarios/id
            var response = await _httpClient.GetAsync(_urlApiComments + "/mascota/" + petId);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateComment(CommentViewModel commentViewModel)
        {
            throw new NotImplementedException();
        }
    }
}

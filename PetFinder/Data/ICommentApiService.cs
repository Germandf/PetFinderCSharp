using PetFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PetFinder.Areas.Identity;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace PetFinder.Data
{
    public interface ICommentApiService
    {
        public Task<HttpResponseMessage> CreateComment(int petId, ApplicationUser user, CommentViewModel commentViewModel);

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

        public async Task<HttpResponseMessage> CreateComment(int petId, ApplicationUser user, CommentViewModel commentViewModel)
        {
            var comment = commentViewModel.ConvertToComment();
            comment.PetId = petId;
            comment.UserId = user.Id;
            // Preparo content del post
            var json = JsonConvert.SerializeObject(comment);
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // POST api/comentarios
            var response = await _httpClient.PostAsync(_urlApiComments, byteContent);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteComment(CommentViewModel commentViewModel)
        {
            var response = await _httpClient.DeleteAsync(_urlApiComments + "/" + commentViewModel.Id);
            return response;
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

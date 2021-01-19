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
using Microsoft.AspNetCore.Components.Authorization;

namespace PetFinder.Data
{
    public interface ICommentApiService
    {
        public Task<HttpResponseMessage> CreateComment(HttpClient httpClient, CommentViewModel commentViewModel, int petId, ApplicationUser user);

        public Task<HttpResponseMessage> DeleteComment(HttpClient httpClient, CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> GetComments(HttpClient httpClient, int petId);

        public Task<HttpResponseMessage> UpdateComment(HttpClient httpClient, CommentViewModel commentViewModel);
    }

    public class CommentApiService : ICommentApiService
    {
        public static string ErrorBadRequest = "El comentario tenía datos incorrectos, intente nuevamente";
        public static string ErrorConflict = "No se ha podido completar la acción, intenta más tarde";
        public static string ErrorNotFound = "No se encontró el objeto solicitado";
        public static string ErrorUnauthorized = "No tienes permiso para realizar esta acción";
        public static string ErrorUnknown = "Ha ocurrido un error inesperado, ponte en contacto con uno de los administradores";
        private readonly string _urlApiComments;

        public CommentApiService(IConfiguration configuration)
        {
            _urlApiComments = configuration["UrlApiController"] + "comentarios";
        }

        public async Task<HttpResponseMessage> CreateComment(HttpClient httpClient, CommentViewModel commentViewModel, int petId, ApplicationUser user)
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
            var response = await httpClient.PostAsync(_urlApiComments, byteContent);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteComment(HttpClient httpClient, CommentViewModel commentViewModel)
        {
            var response = await httpClient.DeleteAsync(_urlApiComments + "/" + commentViewModel.Id);
            return response;
        }

        public async Task<HttpResponseMessage> GetComments(HttpClient httpClient, int petId)
        {
            // GET api/comentarios/id
            var response = await httpClient.GetAsync(_urlApiComments + "/mascota/" + petId);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateComment(HttpClient httpClient, CommentViewModel commentViewModel)
        {
            var comment = commentViewModel.ConvertToComment();
            // Preparo content del put
            var json = JsonConvert.SerializeObject(comment);
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // PUT api/comentarios/id
            var response = await httpClient.PutAsync(_urlApiComments + "/" + comment.Id, byteContent);
            return response;
        }
    }
}

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PetFinder.Areas.Identity;
using PetFinder.ViewModels;

namespace PetFinder.Data
{
    public interface ICommentApiService
    {
        public Task<HttpResponseMessage> CreateComment(CommentViewModel commentViewModel, int petId, ApplicationUser user);

        public Task<HttpResponseMessage> DeleteComment(CommentViewModel commentViewModel);

        public Task<HttpResponseMessage> GetComments(int petId);

        public void SetJwt(string token);

        public Task<HttpResponseMessage> UpdateComment(CommentViewModel commentViewModel);
    }

    public class CommentApiService : ICommentApiService
    {
        public static string ErrorBadRequest = "El comentario tenía datos incorrectos, intente nuevamente";
        public static string ErrorConflict = "No se ha podido completar la acción, intenta más tarde";
        public static string ErrorNotFound = "No se encontró el objeto solicitado";
        public static string ErrorUnauthorized = "No tienes permiso para realizar esta acción";
        public static string ErrorUnknown = "Ha ocurrido un error inesperado, ponte en contacto con uno de los administradores";

        private readonly string _urlApiComments;
        private readonly HttpClient _httpClient;

        public CommentApiService(IConfiguration configuration)
        {
            _urlApiComments = configuration["UrlApiController"] + "comentarios";
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> CreateComment(CommentViewModel commentViewModel, int petId, ApplicationUser user)
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
            // DELETE api/comentarios/id
            var response = await _httpClient.DeleteAsync(_urlApiComments + "/" + commentViewModel.Id);
            return response;
        }

        public async Task<HttpResponseMessage> GetComments(int petId)
        {
            // GET api/comentarios/id
            var response = await _httpClient.GetAsync(_urlApiComments + "/mascota/" + petId);
            return response;
        }

        public void SetJwt(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<HttpResponseMessage> UpdateComment(CommentViewModel commentViewModel)
        {
            var comment = commentViewModel.ConvertToComment();
            // Preparo content del put
            var json = JsonConvert.SerializeObject(comment);
            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // PUT api/comentarios/id
            var response = await _httpClient.PutAsync(_urlApiComments + "/" + comment.Id, byteContent);
            return response;
        }
    }
}
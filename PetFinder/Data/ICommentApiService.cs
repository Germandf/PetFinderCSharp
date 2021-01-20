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
        /// <summary>
        ///     Creates a Comment
        /// </summary>
        /// <returns>
        ///     An HttpResponseMessage
        /// </returns>
        public Task<HttpResponseMessage> Create(CommentViewModel commentViewModel, int petId, ApplicationUser user);

        /// <summary>
        ///     Deletes a Comment
        /// </summary>
        /// <returns>
        ///     An HttpResponseMessage
        /// </returns>
        public Task<HttpResponseMessage> Delete(CommentViewModel commentViewModel);

        /// <summary>
        ///     Gets all Comments with their referenced User
        /// </summary>
        /// <returns>
        ///     An HttpResponseMessage
        /// </returns>
        public Task<HttpResponseMessage> GetAll(int petId);

        /// <summary>
        ///     Sets the Jwt in the current HttpClient
        /// </summary>
        public void SetJwt(string token);

        /// <summary>
        ///     Updates a Comment
        /// </summary>
        /// <returns>
        ///     An HttpResponseMessage
        /// </returns>
        public Task<HttpResponseMessage> Update(CommentViewModel commentViewModel);
    }

    public class CommentApiService : ICommentApiService
    {
        public const string ErrorBadRequest = "El comentario tenía datos incorrectos, intente nuevamente";
        public const string ErrorConflict = "No se ha podido completar la acción, intenta más tarde";
        public const string ErrorNotFound = "No se encontró el objeto solicitado";
        public const string ErrorUnauthorized = "No tienes permiso para realizar esta acción";
        public const string ErrorUnknown = "Ha ocurrido un error inesperado, ponte en contacto con uno de los administradores";

        private readonly string _urlApiComments;
        private readonly HttpClient _httpClient;

        public CommentApiService(IConfiguration configuration)
        {
            _urlApiComments = configuration["UrlApiController"] + "comentarios";
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> Create(CommentViewModel commentViewModel, int petId, ApplicationUser user)
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

        public async Task<HttpResponseMessage> Delete(CommentViewModel commentViewModel)
        {
            // DELETE api/comentarios/id
            var response = await _httpClient.DeleteAsync(_urlApiComments + "/" + commentViewModel.Id);
            return response;
        }

        public async Task<HttpResponseMessage> GetAll(int petId)
        {
            // GET api/comentarios/id
            var response = await _httpClient.GetAsync(_urlApiComments + "/mascota/" + petId);
            return response;
        }

        public void SetJwt(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<HttpResponseMessage> Update(CommentViewModel commentViewModel)
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
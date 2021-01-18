using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PetFinder.Helpers;

namespace PetFinder.Data
{
    public interface IAuthJwtService
    {
        /// <summary>
        ///     Does a POST method to /api/auth with the User's email and password to obtain its JWT
        /// </summary>
        /// <returns>
        ///     The User's temporary Token if the data was correct or a list of errors in case it was not
        /// </returns>
        Task<GenericResult<string>> GetJwt(string email, string password);
    }

    public class AuthJwtService : IAuthJwtService
    {
        public AuthJwtService(IConfiguration Configuration)
        {
            _configuration = Configuration;
            _httpClient = new HttpClient();
            _urlApiAuth = _configuration["UrlApiController"] + "auth";
        }

        public async Task<GenericResult<string>> GetJwt(string email, string password)
        {
            var user = new LoginModel {Email = email, Password = password};
            var jsonUser = JsonConvert.SerializeObject(user);
            // Preparo content del post
            var bufferUser = Encoding.UTF8.GetBytes(jsonUser);
            var byteContent = new ByteArrayContent(bufferUser);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = new GenericResult<string>();
            try
            {
                // POST api/auth
                var response = await _httpClient.PostAsync(_urlApiAuth, byteContent);
                if (response.IsSuccessStatusCode) // Deberia devolver algo así {token: [jwt]}
                {
                    var source = await response.Content.ReadAsStringAsync();
                    var tokenObj = JObject.Parse(source);
                    result.value = Convert.ToString(tokenObj["token"]);
                }
                else
                {
                    result.AddError(response.StatusCode.ToString());
                }
            }
            catch (HttpRequestException exception)
            {
                result.AddError("Error al establecer la conexión. Código de error: " + exception);
            }

            return result;
        }

        #region

        private readonly IConfiguration _configuration;
        private HttpClient _httpClient { get; }
        private string _urlApiAuth { get; }

        #endregion
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
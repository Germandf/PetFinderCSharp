using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PetFinder.Data.Interfaces;
using PetFinder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetFinder.Data
{
    public class AuthJwtService : IAuthJwtService
    {
        #region 
        private IConfiguration _configuration;
        private HttpClient httpClient { get; set; }
        private string URLApiAuth { get; set; }
        #endregion

        public AuthJwtService(IConfiguration Configuration)
        {
            _configuration = Configuration;
            httpClient  = new HttpClient();
            URLApiAuth = _configuration["UrlApiController"] + "auth";
        }
        public async Task<GenericResult<string>> GetJwt(string email, string password)
        {
            LoginModel user = new LoginModel { Email = email, Password = password };
            var jsonUser = JsonConvert.SerializeObject(user);
            // Preparo content del post
            var bufferUser = System.Text.Encoding.UTF8.GetBytes(jsonUser);
            var byteContent = new ByteArrayContent(bufferUser);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            GenericResult<string> result = new GenericResult<string>();

            try
            {
                // POST api/auth
                HttpResponseMessage response = await httpClient.PostAsync(URLApiAuth, byteContent);
                if (response.IsSuccessStatusCode)// Deberia devolver algo así {token: [jwt]}
                {
                    string source = await response.Content.ReadAsStringAsync();
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
                result.AddError("Error al establecer la conexión");
            }
            return result;
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

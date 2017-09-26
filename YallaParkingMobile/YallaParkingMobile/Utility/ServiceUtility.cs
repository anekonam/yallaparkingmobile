using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YallaParkingMobile.Model;

namespace YallaParkingMobile.Utility {

    public static class ServiceUtility {

        private static HttpClient client;

        private static string webUrl = "http://yallaparking-new.insiso.co.uk";
        private static void InitHttpClient() {
            client = new HttpClient(new HttpClientHandler {
                ClientCertificateOptions = ClientCertificateOption.Automatic                
            });

            client.BaseAddress = new Uri(webUrl);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = PropertyUtility.GetValue("Token");

            if (!string.IsNullOrWhiteSpace(token)) {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("bearer {0}", token));
            }
        }        

        public static async Task<bool> RequestToken(string username, string password) {
            InitHttpClient();

            try {
                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                var response = await client.PostAsync("/token", content);

                if (response.IsSuccessStatusCode) {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<TokenModel>(tokenResponse);

                    PropertyUtility.SetValue("Token", token.Access_Token);
                    return true;
                }

                return false;
            } catch {
                return false;
            }            
        }

        public static async Task<HttpResponseMessage> Login(LoginModel model) {
            InitHttpClient();

            try {
                var response = await client.PostAsync("/api/account/login", model.AsJson());

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Forbidden) {
                    var result = await RequestToken(model.EmailAddress, model.Password);
                    response.StatusCode = result ? response.StatusCode : System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }

                return response;
            } catch {
                return null;
            }            
        }

        public static async Task<bool> Register(RegisterModel model) {
            InitHttpClient();

            try {                
                var response = await client.PostAsync("/api/account/register", model.AsJson());

                if (response.IsSuccessStatusCode) {
                    var result = await RequestToken(model.EmailAddress, model.Password);
                    return result;
                }

                return false;
            } catch{
                return false;
            }            
        }

        public static async Task<bool> RegisterConfirm(string code) {
            InitHttpClient();

            try {
                var response = await client.PostAsync("/api/account/registerConfirm", code.AsJson()); 
                
                if (response.IsSuccessStatusCode) {
                    return true;
                }

                return false;
            } catch {
                return false;
            }            
        }

        private static StringContent AsJson(this object o) => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

    }   
}

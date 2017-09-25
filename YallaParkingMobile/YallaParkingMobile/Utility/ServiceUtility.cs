using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YallaParkingMobile.Model;

namespace YallaParkingMobile.Utility {

    public static class ServiceUtility {

        private static HttpClient client;

#if DEBUG
        private static string webUrl = "http://yallaparking-new.insiso.co.uk";
#else
        private static string webUrl = Settings.Default.WebUrl;        
#endif
        private static void InitHttpClient() {
            client = new HttpClient(new HttpClientHandler {
                ClientCertificateOptions = ClientCertificateOption.Automatic                
            });

            client.BaseAddress = new Uri(webUrl);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = PropertyUtility.GetValue("token");

            if (!string.IsNullOrWhiteSpace(token)) {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("bearer {0}", token));
            }
        }

        public static async Task<HttpResponseMessage> Login(LoginModel model) {
            InitHttpClient();

            try {
                var result = await client.PostAsync("/api/account/login", model.AsJson());
                return result;
            } catch {
            }

            return null;
        }

        public static async Task<bool> Register(RegisterModel model) {
            InitHttpClient();

            try {                
                var result = await client.PostAsync("/api/account/register", model.AsJson());

                if (result.IsSuccessStatusCode) {
                    return true;
                }
            } catch{                
            }

            return false;
        }

        public static async Task<bool> RegisterConfirm(string code) {
            InitHttpClient();

            try {
                var result = await client.PostAsync("/api/account/registerConfirm", code.AsJson());                
                if (result.IsSuccessStatusCode) {
                    return true;
                }
            } catch {                
            }

            return false;
        }

        private static StringContent AsJson(this object o) => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

    }   
}

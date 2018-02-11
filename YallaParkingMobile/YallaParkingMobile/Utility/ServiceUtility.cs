using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YallaParkingMobile.Model;
using System.Net;

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

        public static async Task<HttpResponseMessage> Register(RegisterModel model) {
            InitHttpClient();

            try {                
                var response = await client.PostAsync("/api/account/register", model.AsJson());

                if (response.IsSuccessStatusCode) {
                    var result = await RequestToken(model.EmailAddress, model.Password);
					response.StatusCode = result ? response.StatusCode : System.Net.HttpStatusCode.Unauthorized;
					return response;
                }

                return response;
            } catch{
                return null;
            }            
        }

		public static async Task<HttpResponseMessage> ResetPassword(string emailAddress) {
			InitHttpClient();

			try {
                var model = new {
                    EmailAddress = emailAddress
                };

				var response = await client.PostAsync("/api/account/resetPassword", model.AsJson());

				if (response.IsSuccessStatusCode) {					
					return response;
				}

				return response;
			} catch {
				return null;
			}
		}

		public static async Task<bool> Verify(string code) {
            InitHttpClient();

            var verify = new {
                RegisterCode = code
            };

            try {
                var response = await client.PostAsync("/api/account/verify", verify.AsJson());
                
                if (response.IsSuccessStatusCode) {
                    return true;
                }

                return false;
            } catch {
                return false;
            }            
        }

		public static async Task<ProfileModel> Profile()
		{
			InitHttpClient();

			try
			{
				var response = await client.GetAsync("/api/account/profile");

				if (response.IsSuccessStatusCode)
				{
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<ProfileModel>(profileResponse);
				}
			}
			catch
			{
				return null;
			}

            return null;
		}

        public static async Task<ProfileModel> UpdateProfile(ProfileModel model) {
            InitHttpClient();

            try {
                var response = await client.PostAsync("/api/account/update", model.AsJson());

                if (response.IsSuccessStatusCode) {
                    var profileResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ProfileModel>(profileResponse);
                }
            } catch {
                return null;
            }

            return null;
        }

        public static async Task<List<PropertyModel>> Search(SearchModel search) {
            InitHttpClient();

            try {
                var response = await client.PostAsync("/api/parking/search", search.AsJson());

                if (response.IsSuccessStatusCode) {
                    var searchResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<PropertyModel>>(searchResponse);
                }
            } catch {
                return null;
            }

            return null;
        }

        public static async Task<List<string>> PropertyAreas() {
            InitHttpClient();

            try {
                var response = await client.GetAsync("/api/parking/properties");

                if (response.IsSuccessStatusCode) {
                    var propertyResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<string>>(propertyResponse);
                }
            } catch {
                return null;
            }

            return null;
        }

        public static async Task<DiscountModel> DiscountCode(string code) {
            InitHttpClient();

            var discountModel = new DiscountModel{
                DiscountCode = code
            };

            try {
                var response = await client.PostAsync("/api/parking/discount", discountModel.AsJson());

                if (response.IsSuccessStatusCode) {
                    var discountResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DiscountModel>(discountResponse);
                }

                return discountModel;
            } catch {
                return discountModel;
            }
        }

        private static StringContent AsJson(this object o) => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

		public static async Task<List<UserCarModel>> GetUserCars() {
			InitHttpClient();

			try {
                var response = await client.GetAsync("/api/account/getUserCars");

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<List<UserCarModel>>(profileResponse);
				}
			} catch {
				return null;
			}

			return null;
		}

		public static async Task<UserCarModel> UpdateUserCar(UserCarModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/account/updateUserCar", model.AsJson());

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<UserCarModel>(profileResponse);
				}
			} catch {
				return null;
			}

			return null;
		}

		public static async Task<bool> DeleteUserCar(UserCarModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/account/deleteUserCar", model.AsJson());

                if (response.IsSuccessStatusCode) {					
                    return true;
				}
			} catch {
                return false;
			}

            return false;
		}

		public static async Task<List<UserCardModel>> GetUserCards() {
			InitHttpClient();

			try {
				var response = await client.GetAsync("/api/account/getUserCards");

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<List<UserCardModel>>(profileResponse);
				}
			} catch {
				return null;
			}

			return null;
		}

		public static async Task<UserCardModel> UpdateUserCard(UserCardModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/account/updateUserCard", model.AsJson());

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<UserCardModel>(profileResponse);
                } 
			} catch {
				return null;
			}

			return null;
		}

        public static async Task<HttpResponseMessage> DeleteUserCard(UserCardModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/account/deleteUserCard", model.AsJson());
                return response;
			} catch {
				return null;
			}
		}

        public static async Task<HttpResponseMessage> Book(BookingModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/parking/book", model.AsJson());
                return response;
            } catch(Exception ex) {
                return null;
			}
		}

		public static async Task<bool> Update(BookingModel model) {
			InitHttpClient();

			try {
				var response = await client.PostAsync("/api/parking/update", model.AsJson());

				if (response.IsSuccessStatusCode) {
					return true;
				}
            } catch(System.Exception ex) {
                return false;
			}

            return false;
		}

		public static async Task<bool> Entry(int propertyId, DateTime startDate) {
			InitHttpClient();

			try {
                var model = new {
                    PropertyId = propertyId,
                    StartDate = startDate
                };

				var response = await client.PostAsync("/api/parking/entry", model.AsJson());

				if (response.IsSuccessStatusCode) {
					return true;
				}
			} catch {
				return false;
			}

			return false;
		}

		public static async Task<bool> Exit(int propertyId) {
			InitHttpClient();

			try {
				var model = new {
					PropertyId = propertyId
				};

				var response = await client.PostAsync("/api/parking/exit", model.AsJson());

				if (response.IsSuccessStatusCode) {
					return true;
				}
			} catch {
				return false;
			}

			return false;
		}

		public static async Task<bool> Cancel(int propertyParkingId) {
			InitHttpClient();

			try {
				var model = new {
					PropertyParkingId = propertyParkingId
				};

				var response = await client.PostAsync("/api/parking/cancel", model.AsJson());

				if (response.IsSuccessStatusCode) {
					return true;
				}
			} catch {
				return false;
			}

			return false;
		}

		public static async Task<bool> Validate(int propertyParkingId, int userId) {
			InitHttpClient();

			try {
				var model = new {
                    PropertyParkingId = propertyParkingId,
                    UserId = userId
				};

				var response = await client.PostAsync("/api/parking/validate", model.AsJson());

				if (response.IsSuccessStatusCode) {
					return true;
				}
			} catch {
				return false;
			}

			return false;
		}


		public static async Task<List<BookingModel>> GetBookings() {
			InitHttpClient();

			try {
				var response = await client.GetAsync("/api/account/bookings");

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<List<BookingModel>>(profileResponse);
				}
			} catch {
				return null;
			}

			return null;
		}

		public static async Task<BookingModel> GetBooking(int propertyParkingId) {
			InitHttpClient();

			try {
                var response = await client.GetAsync("/api/account/booking/"+propertyParkingId);

				if (response.IsSuccessStatusCode) {
					var profileResponse = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<BookingModel>(profileResponse);
				}
			} catch {
				return null;
			}

			return null;
		}


	}   
}

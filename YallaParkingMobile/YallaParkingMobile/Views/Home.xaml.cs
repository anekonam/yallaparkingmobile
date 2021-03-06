﻿using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using Telerik.XamarinForms.Input.AutoComplete;
using System.IO;
using DurianCode.PlacesSearchBar;
using TK.CustomMap;

namespace YallaParkingMobile {
    public partial class Home : ContentPage {

        private Xamarin.Forms.Maps.Position mapPosition;
        private string GooglePlacesApiKey = "AIzaSyCz3rmlS7ThnvZJ0oq-GYmlWRSbceEyQrE";
        private TKCustomMapPin currentPin = null;
        private IGeolocator locator = null;
        private bool loading = false;

        public Home() {
            InitializeComponent();
        }

        public Home(HomeModel model) {
            this.Model = model;

            InitializeComponent();

            UpdateButtonStates();

            UpdateCurrentLocation();

            NavigationPage.SetHasNavigationBar(this, false);

            Analytics.TrackEvent("Viewing Home Page");

            var dubai = new Xamarin.Forms.Maps.Position(25.2048, 55.2708);
            var mapView = new TKCustomMap(MapSpan.FromCenterAndRadius(dubai, Distance.FromMiles(0.3)));

            PropertyUtility.SetValue("LoggedIn", "true");
            PropertyUtility.RemoveKey("query");
            PropertyUtility.RemoveKey("hours");             

            SearchDate.Date = DateTime.Now;
            SearchDate.MinimumDate = DateTime.Now;

            var maxHours = (int)Math.Ceiling((DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalHours);
            maxHours = maxHours >= 8 ? 8 : maxHours;
            ParkLaterHoursSlider.Maximum = maxHours >= 2 ? maxHours : 2;
            SearchTime.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

			Search.ApiKey = GooglePlacesApiKey;
            Search.Type = PlaceType.All;
            Search.Bias = new LocationBias(25.2048, 55.2708, 4200000);
			Search.PlacesRetrieved += Search_Bar_PlacesRetrieved;
			Search.TextChanged += Search_Bar_TextChanged;
			Search.MinimumSearchText = 2;
			Search.Placeholder = "Search parking area or building...";
			PlaceList.ItemSelected += Results_List_ItemSelected;           

            SearchDate.Effects.Add(Effect.Resolve(("Effects.BorderlessEffect")));
            SearchTime.Effects.Add(Effect.Resolve(("Effects.BorderlessEffect")));
            HoursSlider.Effects.Add(Effect.Resolve(("Effects.SliderEffect")));
            ParkLaterHoursSlider.Effects.Add(Effect.Resolve(("Effects.SliderEffect")));

			SearchTime.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == TimePicker.TimeProperty.PropertyName) {
                    var maximumHours = 24 - this.SearchTime.Time.Hours;
                    maximumHours = maximumHours >= 8 ? 8 : maximumHours;
					ParkLaterHoursSlider.Maximum = maximumHours >= 2 ? maximumHours : 2;

					if ((int)ParkLaterHoursSlider.Value >= 8) {
						ParkLaterHours.Text = "All Day";
					} else {
						ParkLaterHours.Text = string.Format("{0} hours", ParkLaterHoursSlider.Value);
					}
				}
			};
        }

        void Search_Bar_PlacesRetrieved(object sender, AutoCompleteResult result) {
             PlaceList.ItemsSource = result.AutoCompletePlaces;

            if (result.AutoCompletePlaces != null && result.AutoCompletePlaces.Count > 0) {
                PlaceList.IsVisible = true;
                PlaceFrame.IsVisible = true;
            }
        }

        void Search_Bar_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrWhiteSpace(e.NewTextValue)) {
                PlaceList.IsVisible = false;
                PlaceFrame.IsVisible = false;
            } else {
                PlaceList.IsVisible = true;
                PlaceFrame.IsVisible = true;
            }
        }

        public Place Place { get; set; }

        async void Results_List_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            if (e.SelectedItem == null)
                return;

            var prediction = (AutoCompletePrediction)e.SelectedItem;
            PlaceList.SelectedItem = null;
            PlaceList.IsVisible = false;
            PlaceFrame.IsVisible = false;

            var place = await Places.GetPlace(prediction.Place_ID, GooglePlacesApiKey);
            var pins = this.Map.CustomPins !=null && this.Map.CustomPins.Any() ? this.Map.CustomPins.ToList() : new List<TKCustomMapPin>();
			var placePin = pins.FirstOrDefault(p => p.BindingContext.GetType() == typeof(Place));                      

            if (place != null) {
                this.Place = place;
                var position = new Xamarin.Forms.Maps.Position(place.Latitude, place.Longitude);
                Map.MapCenter = position;
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(Map.MapCenter, Distance.FromMeters(800)));

                if (this.Map.CustomPins !=null && !this.Map.CustomPins.Any(p => p.Position.Latitude == place.Latitude && p.Position.Longitude == place.Longitude)) {
                    if (placePin != null && placePin.BindingContext.GetType() == typeof(Place)) {
                        placePin.BindingContext = place;
                        placePin.Title = place.Name;
                        placePin.Position = new Xamarin.Forms.Maps.Position(place.Latitude, place.Longitude);
                        placePin.Image = ImageSource.FromFile("place_holder.png");
                        placePin.ShowCallout = true;
                    } else {
                        pins.Add(new TKCustomMapPin {
                            BindingContext = place,
                            Position = new Xamarin.Forms.Maps.Position(place.Latitude, place.Longitude),
                            Title = place.Name,
                            Image = ImageSource.FromFile("place_holder.png"),
                            ShowCallout = true
                        });
                    }

                    this.Map.CustomPins = pins;
                } else{
                    if(placePin!=null){
                        pins.Remove(placePin);
                        this.Map.CustomPins = pins;
                    }
                }
            }

            this.Search.Unfocus();
        }

        void Handle_Tapped(object sender, System.EventArgs e) {
            this.SearchDate.Focus();
        }

        public HomeModel Model {
            get {
                return (HomeModel)this.BindingContext;
            }
            set {
                this.BindingContext = value;
            }
        }

        private void UpdateButtonStates() {
            if (this.Model.ParkNow) {
                this.ParkNowButton.BackgroundColor = Color.White;
                this.ParkNowButton.BorderWidth = 1;
                this.ParkNowButton.BorderColor = Color.FromHex("#ff8e30");
                this.ParkNowButton.TextColor = Color.FromHex("#ff8e30");
                this.ParkNowButton.FontAttributes = FontAttributes.Bold;

                this.ParkLaterButton.BackgroundColor = Color.FromHex("#ff8e30");
                this.ParkLaterButton.BorderWidth = 0;
                this.ParkLaterButton.TextColor = Color.White;
                this.ParkLaterButton.FontAttributes = FontAttributes.None;

                this.SearchDateTime.IsVisible = false;
                this.ParkLaterSlider.IsVisible = false;
                this.ParkNowSlider.IsVisible = true;
                this.SearchFrame.HeightRequest = 110;

            } else {
                this.ParkNowButton.BackgroundColor = Color.FromHex("#ff8e30");
                this.ParkNowButton.BorderWidth = 0;
                this.ParkNowButton.TextColor = Color.White;
                this.ParkNowButton.FontAttributes = FontAttributes.None;

                this.ParkLaterButton.BackgroundColor = Color.White;
                this.ParkLaterButton.BorderWidth = 1;
                this.ParkLaterButton.BorderColor = Color.FromHex("#ff8e30");
                this.ParkLaterButton.TextColor = Color.FromHex("#ff8e30");
                this.ParkLaterButton.FontAttributes = FontAttributes.Bold;

                this.SearchDateTime.IsVisible = true;
                this.ParkLaterSlider.IsVisible = true;
                this.ParkNowSlider.IsVisible = false;
                this.SearchFrame.HeightRequest = 150;
            }
        }

        public ProfileModel UserProfile { get; set; }

        async void Handle_Appearing(object sender, System.EventArgs e) {
            var loginValid = await ServiceUtility.Check();

            if (!loginValid) {
				PropertyUtility.RemoveKey("Token");
				PropertyUtility.RemoveKey("LoggedIn");
				await Navigation.PushAsync(new Login());
                return; 
            } 

			var lc = CrossGeolocator.Current;
		     	lc.DesiredAccuracy = 100;

			if (lc.IsGeolocationEnabled == false) {
				await DisplayAlert("Notification", "In order to use this application you must be able to share your location.", "Ok");
			} else {
                try {
                    var position = await lc.GetPositionAsync(TimeSpan.FromMilliseconds(1000));
                    var latitude = position.Latitude;
                    var longitude = position.Longitude;
                    //await DisplayAlert("Current Location ", "Latitude is " + latitude + " Longitude is " + longitude, "Ok");
                } catch (Exception ex) {
                    await DisplayAlert("Location Required", "In order to use this application you must be able to share your location.", "Ok");
                }
            }
           

            var query = PropertyUtility.GetValue("query");
            var hours = PropertyUtility.GetValue("hours");

            if (!string.IsNullOrWhiteSpace(hours)) {
                this.ParkLaterHoursSlider.Value = int.Parse(hours);
            }

            await LoadData();

            var profile = await ServiceUtility.Profile();
            this.ProfileName.Text = profile.Name;
            this.UserProfile = profile;

            if (!string.IsNullOrWhiteSpace(profile.ProfilePicture)) {
                var profileImage = !string.IsNullOrWhiteSpace(profile.ProfilePicture) && profile.ProfilePicture.Contains(",") ? profile.ProfilePicture.Split(',')[1] : profile.ProfilePicture;
                this.ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
            }
        }

        async void Handle_Disappearing(object sender, System.EventArgs e) {
            locator = CrossGeolocator.Current;
            locator.PositionChanged += Current_PositionChanged;

            if (locator.IsListening) {
                await locator.StopListeningAsync();
            }
        }

        async Task LoadData() {
            if (!loading) {

                if (this.Model != null) {
                    this.Model.SelectedProperty = null;
                }

                var startDate = this.SearchDate.Date;
                startDate.Date.Add(this.SearchTime.Time);

                var search = new SearchModel {
                    StartDate = startDate,
                    Hours = (int)this.ParkLaterHoursSlider.Value
                };

                PropertyUtility.SetValue("query", search.Name);
                PropertyUtility.SetValue("hours", search.Hours.ToString());

                loading = true;

                var properties = await ServiceUtility.Search(search);

                this.Map.Pins.Clear();

                var pins = new List<TKCustomMapPin>();

                if (properties != null) {
                    foreach (var property in properties) {
                        var pin = new TKCustomMapPin {
                            BindingContext = property,
                            Position = new Xamarin.Forms.Maps.Position(property.Latitude ?? 0.0, property.Longitude ?? 0.0),
                            Anchor = new Point(0.5, 1),
                            Image = this.Model.ParkNow == true ? ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false",
                                                                                                           (int)HoursSlider.Value >= 1 ? (int)property.ShortTermParkingFullDayPrice : (int)property.ShortTermParkingPrice, (int)HoursSlider.Value >= 1 ? "day" : "hr"))) :
                                        ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false",
																			  (int)ParkLaterHoursSlider.Value >= 8 ? (int)property.ShortTermParkingFullDayPrice : (int)property.ShortTermParkingPrice, (int)ParkLaterHoursSlider.Value >= 8 ? "day" : "hr"))),
                            ShowCallout = false
                        };

                        pins.Add(pin);
                    }

                    this.Map.CustomPins = pins;

				} else if (this.Place != null) {
					var pin = new TKCustomMapPin {
						BindingContext = this.Place,
						Position = new Xamarin.Forms.Maps.Position(this.Place.Latitude, this.Place.Longitude),
						Title = this.Place.Name,
						Image = ImageSource.FromFile("place_holder.png"),
						ShowCallout = true
					};

					pins.Add(pin);

					this.Map.CustomPins = pins;
				}
                 

                if (!Map.IsVisible) {
                    Map.IsVisible = true;
                }

                loading = false;
            }
        }   

        void Handle_PinSelected(object sender, TK.CustomMap.TKGenericEventArgs<TK.CustomMap.TKCustomMapPin> e) {
            if (e.Value.BindingContext.GetType() == typeof(PropertyModel)){
                if (currentPin != null) {
                    var customPin = this.Map.CustomPins.FirstOrDefault(p => p == currentPin);

                    if (customPin != null) {
                        var pinProperty = (PropertyModel)customPin.BindingContext;
                        if(this.Model.ParkNow == true){
                        customPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false", (int)HoursSlider.Value >= 1 ? (int)pinProperty.ShortTermParkingFullDayPrice : (int)pinProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 1 ? "day" : "hr")));
                        } else{
                           customPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false", (int)ParkLaterHoursSlider.Value >= 8 ? (int)pinProperty.ShortTermParkingFullDayPrice : (int)pinProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 8 ? "day" : "hr"))); 
                        }
                    }
                }

                if (e.Value != currentPin) {
                    var property = (PropertyModel)this.Map.SelectedPin.BindingContext;
                    this.Model.SelectedProperty = property;

                    if (this.Model.ParkNow == true) {
                        this.Map.SelectedPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=true", (int)HoursSlider.Value >= 1 ? (int)this.Model.SelectedProperty.ShortTermParkingFullDayPrice : (int)this.Model.SelectedProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 1 ? "day" : "hr")));
                    }else{
                        this.Map.SelectedPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=true", (int)ParkLaterHoursSlider.Value >= 8 ? (int)this.Model.SelectedProperty.ShortTermParkingFullDayPrice : (int)this.Model.SelectedProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 8 ? "day" : "hr")));
                    }
                    currentPin = this.Map.SelectedPin;

                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Value.Position, Distance.FromMeters(400)));
                } else {
                    this.Map.SelectedPin = null;
                    this.Model.SelectedProperty = null;
                    currentPin = null;
                }
            }
        }

        private void Map_MapClicked(object sender, TKGenericEventArgs<Xamarin.Forms.Maps.Position> e) {
            if (currentPin != null) {
                var customPin = this.Map.CustomPins.FirstOrDefault(p => p == currentPin);
				
                if (customPin != null) {
					var pinProperty = (PropertyModel)customPin.BindingContext;
                    if (this.Model.ParkNow == true) {
                        customPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false", (int)HoursSlider.Value >= 1 ? (int)pinProperty.ShortTermParkingFullDayPrice : (int)pinProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 1 ? "day" : "hr")));
                    } else{
                        customPin.Image = ImageSource.FromUri(new Uri(string.Format("https://www.yallaparking.com/image/pin?price={0}&unit={1}&selected=false", (int)ParkLaterHoursSlider.Value >= 8 ? (int)pinProperty.ShortTermParkingFullDayPrice : (int)pinProperty.ShortTermParkingPrice, (int)HoursSlider.Value >= 8 ? "day" : "hr")));
                    }
				}

                this.Map.SelectedPin = null;
                this.Model.SelectedProperty = null;
            }            
        }

        private void Location_Clicked(object sender, EventArgs e) {
            UpdateCurrentLocation();
        }


        private void UpdateCurrentLocation() {
            this.GetCurrentLocation().ContinueWith(response => {
                var position = response.Result;
                mapPosition = position != null ?
                    new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude) :
                    new Xamarin.Forms.Maps.Position(25.1985, 55.2796);

                Device.StartTimer(TimeSpan.FromMilliseconds(500), () => {
                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(mapPosition, Distance.FromMeters(Model.ParkNow ? 300 : 400)));
                    return false;
                });
            });
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e) {
            if (string.IsNullOrWhiteSpace(this.Search.Text)) {
                UpdateCurrentLocation();
            }
        }

        private async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentLocation() {
            Plugin.Geolocator.Abstractions.Position position = null;

            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location)) {
                    await DisplayAlert("Location Required", "In order to use this application you must be able to share your location.", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location)) {
                    status = results[Permission.Location];
                }

                var locator = CrossGeolocator.Current;

                if (status == PermissionStatus.Granted) {
                    locator.DesiredAccuracy = 100;

                    position = await locator.GetLastKnownLocationAsync();

                    if (position != null) {
                        return position;
                    }

                    if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled) {
                        return null;
                    }

                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            if (position == null) {
                return position;
            }

            var output = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                position.Timestamp, position.Latitude, position.Longitude,
                position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

            Analytics.TrackEvent(output);

            return position;
        }

        void searchBarTextChanged(object sender, TextChangedEventArgs textChangedEventArgs) {
            // Has Backspace or Cancel has been pressed?
            if (textChangedEventArgs.NewTextValue == string.Empty) {
                // Cancel pressed
                if (textChangedEventArgs.OldTextValue.Length > 1) { } else { }

            }
        }

        private async void Next_Clicked(object sender, System.EventArgs e) {
            var property = this.Model.SelectedProperty;
            var parkNow = this.Model.ParkNow;
            var allDay = this.Model.AllDay;

            if (Model.ParkNow) {
                property.StartDate = DateTime.Now;
                property.Hours = (int)this.HoursSlider.Value;
            } else{
                property.StartDate = SearchDate.Date.Add(SearchTime.Time);
                property.Hours = (int)this.ParkLaterHoursSlider.Value;
            }

            var bookParking = new BookingPreview(new BookParkingModel(property, parkNow, allDay,!SearchDateTime.IsVisible));
            await Navigation.PushAsync(bookParking);
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Menu.IsOpen = !Menu.IsOpen;
        }

        private void ParkNow_Clicked(object sender, EventArgs e) {
            this.Model.ParkNow = true;
            UpdateButtonStates();

			if (string.IsNullOrWhiteSpace(this.Search.Text)) {
				UpdateCurrentLocation();
			}
        }

        private void ParkLater_Clicked(object sender, EventArgs e) {
            this.Model.ParkNow = false;
            UpdateButtonStates();

            if (string.IsNullOrWhiteSpace(this.Search.Text)) {
                UpdateCurrentLocation();
            }
        }

        private async void FindParking_Clicked(object sender, EventArgs e) {
            var model = new HomeModel();
            var home = new Home(model);

            await Navigation.PushAsync(home);
        }

        private async void MyBookings_Clicked(object sender, EventArgs e) {
            var model = new BookingsModel();
            var bookings = new Bookings(model);

            await Navigation.PushAsync(bookings);
        }

        private async void MyProfile_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Profile());
        }

        private async void Invite_Clicked(object sender, EventArgs e) {
            var inviteCode = new Invite();
			inviteCode.BindingContext = this.UserProfile;
			await Navigation.PushAsync(inviteCode);
        }

        private async void HoursSlider_ValueChanged(object sender, ValueChangedEventArgs e) {

			var newStep = Math.Round(e.NewValue / 1.0);
			HoursSlider.Value = newStep * 1.0;

			if ((int)HoursSlider.Value >= 1) {
                this.Model.AllDay = true;
			} else {
                this.Model.AllDay = false;
			}

                await LoadData();
        }

		private async void ParkLaterHoursSlider_ValueChanged(object sender, ValueChangedEventArgs e) {

			var hoursText = ParkLaterHours.Text;

			var newStep = Math.Round(e.NewValue / 1.0);
            ParkLaterHoursSlider.Value = newStep * 1.0;

			if ((int)ParkLaterHoursSlider.Value >= 8) {
				ParkLaterHours.Text = "All Day";
                this.Model.AllDay = true;
			} else {
				ParkLaterHours.Text = string.Format("{0} hours", ParkLaterHoursSlider.Value);
                this.Model.AllDay = false;
			}

            if(hoursText!=ParkLaterHours.Text){
                await LoadData();
            }
		}

        async void DatePicker_DateSelected(object sender, DateChangedEventArgs e) {
            await LoadData();
        }
    }
}

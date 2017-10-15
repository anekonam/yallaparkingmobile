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

namespace YallaParkingMobile {
    public partial class Home : ContentPage {
      
        private Xamarin.Forms.Maps.Position mapPosition;

        public Home() {
            InitializeComponent();
        }

        public Home (HomeModel model){
            this.Model = model;

            InitializeComponent();

			if (this.Model.ParkNow) {
				this.Title = "Find Parking Now";
				this.ParkLaterButton.Opacity = 0.8;
			} else {
				this.Title = "Find Parking Later";
				this.ParkNowButton.Opacity = 0.8;
			}

			Analytics.TrackEvent("Viewing Home Page");

			PropertyUtility.SetValue("LoggedIn", "true");

			UpdateCurrentLocation();

			CrossGeolocator.Current.PositionChanged += Current_PositionChanged;

			SearchDate.Date = DateTime.Now;
            SearchDate.MinimumDate = DateTime.Now;

            HoursSlider.Maximum = (int)Math.Ceiling((DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalHours);
            SearchTime.Time = TimeSpan.FromHours(DateTime.Now.Hour < 23 ? DateTime.Now.Hour + 1 : 0);

			HoursSlider.Effects.Add(Effect.Resolve(("Effects.SliderEffect")));
        }

        public HomeModel Model{
            get{
                return (HomeModel)this.BindingContext;
            } set{
                this.BindingContext = value;
            }
        }

		async void Handle_Appearing(object sender, System.EventArgs e) {			
            this.Search.ItemsSource = await ServiceUtility.PropertyAreas();
            await LoadData();
		}

        async void Handle_FilteredItemsChanged(object sender, Telerik.XamarinForms.Input.AutoComplete.FilteredItemsChangedEventArgs e) {
            await LoadData(this.Search.Text);
        }

        async Task LoadData(string query = null) {
           var startDate = this.SearchDate.Date.Add(this.SearchTime.Time);

			BusyIndicator.IsBusy = true;

			var search = new SearchModel {
                Name = query,
                StartDate = startDate,
                Hours = (int)this.HoursSlider.Value
            };

            var properties = await ServiceUtility.Search(search);

            this.Map.Pins.Clear();

            if (properties != null) {
                foreach (var property in properties) {
                    var pin = new Pin {
                        BindingContext = property,
                        Position = new Xamarin.Forms.Maps.Position(property.Latitude ?? 0.0, property.Longitude ?? 0.0),
                        Label = string.Format("{0} (AED {1}/hr)", property.Name, property.ShortTermParkingPrice),
                        Address = property.AreaName,
                        Type = PinType.SearchResult,
                    };

                    pin.Clicked += Pin_Clicked;

                    this.Map.Pins.Add(pin);
                }
            }

			var geocoder = new Geocoder();
			var positions = await geocoder.GetPositionsForAddressAsync(query + ", Dubai");

            if(!string.IsNullOrWhiteSpace(query) && this.Map.Pins!=null && this.Map.Pins.Any(p => p.Label.Contains(query))){
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(this.Map.Pins.First(p => p.Label.Contains(query)).Position, Distance.FromMiles(1)));
            } else if (positions.Any()) {
				Map.MoveToRegion(MapSpan.FromCenterAndRadius(positions.First(), Distance.FromMiles(1)));
			}

            BusyIndicator.IsBusy = false;

            if(!Map.IsVisible){
                Map.IsVisible = true;
            }
        }

        private async void Pin_Clicked(object sender, EventArgs e) {
            var pin = (Pin)sender;
            var property = (PropertyModel)pin.BindingContext;

            if (SearchDateTime.IsVisible) {
                property.StartDate = this.SearchDate.Date.Add(this.SearchTime.Time);
            } else{
                property.StartDate = DateTime.UtcNow;
            }

            property.Hours = (int)this.HoursSlider.Value;

            var bookParking = new BookParking(new BookParkingModel(property, !SearchDateTime.IsVisible));
            await Navigation.PushAsync(bookParking);
        }

        async void Search_SuggestionItemSelected(object sender, Telerik.XamarinForms.Input.AutoComplete.SuggestionItemSelectedEventArgs e) {
            var address = e.DataItem.ToString();
            this.Search.Text = address;
            await LoadData(address);                        
        }

        private void UpdateCurrentLocation() {
            this.GetCurrentLocation().ContinueWith(response => {
                var position = response.Result;
                mapPosition = position != null ?
                    new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude) :
                    new Xamarin.Forms.Maps.Position(25.1985, 55.2796);


                Map.MoveToRegion(MapSpan.FromCenterAndRadius(mapPosition, Distance.FromMiles(1)));
            });
        }

        private void Current_PositionChanged(object sender, PositionEventArgs e) {
            UpdateCurrentLocation();
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

        private void Button_Clicked(object sender, EventArgs e) {
            Menu.IsOpen = !Menu.IsOpen;
        }

		private async void ParkNow_Clicked(object sender, EventArgs e) {
			var model = new HomeModel();
			var home = new Home(model);

			await Navigation.PushAsync(home);
		}

		private async void ParkLater_Clicked(object sender, EventArgs e) {
			var model = new HomeModel(false);
			var home = new Home(model);

			await Navigation.PushAsync(home);
		}

        private async void FindParking_Clicked(object sender, EventArgs e) {
			var home = new Home();
			home.BindingContext = new HomeModel();
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
            await Navigation.PushAsync(new Invite());
        }

        private async void Logout_Clicked(object sender, EventArgs e) {
            PropertyUtility.RemoveKey("token");
            await Navigation.PushAsync(new Login());
        }

        private void HoursSlider_ValueChanged(object sender, ValueChangedEventArgs e) {
            var newStep = Math.Round(e.NewValue / 1.0);

            HoursSlider.Value = newStep * 1.0;

            if ((int)HoursSlider.Value == (int)HoursSlider.Maximum) {
                Hours.Text = "All Day";
            } else {
                Hours.Text = string.Format("{0} hours", HoursSlider.Value);
            }
        }

        async void DatePicker_DateSelected(object sender, DateChangedEventArgs e) {
            await LoadData(this.Search.Text);
        }

    }
}

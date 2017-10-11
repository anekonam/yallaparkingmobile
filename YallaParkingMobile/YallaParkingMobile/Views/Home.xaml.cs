using Microsoft.Azure.Mobile.Analytics;
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
        void Handle_Clicked(object sender, System.EventArgs e) {
            throw new NotImplementedException();
        }

        private Xamarin.Forms.Maps.Position mapPosition;

        public Home() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Home Page");

            PropertyUtility.SetValue("LoggedIn", "true");

            UpdateCurrentLocation();

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;            

            SearchDate.Date = DateTime.Now;
            Search.Unfocused += Search_Unfocused;

            HoursSlider.Effects.Add(Effect.Resolve(("Effects.SliderEffect")));
        }

		async void Handle_Appearing(object sender, System.EventArgs e) {
            this.Search.ItemsSource = await ServiceUtility.PropertyAreas();
		}

        async void Search_Unfocused(object sender, FocusEventArgs e) {
            LoadData(Search.Text);
        }

        async Task LoadData(string query = null) {
            BusyIndicator.IsBusy = true;

            var search = new SearchModel {
                Name = query,
                StartDate = this.SearchDate.Date.Add(this.SearchTime.Time),
                Hours = (int)this.HoursSlider.Value
            };

            var properties = await ServiceUtility.Search(search);

            this.Map.Pins.Clear();

            foreach(var property in properties) {
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

            if (this.Map.Pins.Any()) {                
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(this.Map.Pins.First().Position, Distance.FromMiles(1)));
            }

            BusyIndicator.IsBusy = false;           
        }

        private async void Pin_Clicked(object sender, EventArgs e) {
            var pin = (Pin)sender;
            var property = (PropertyModel)pin.BindingContext;

            property.StartDate = this.SearchDate.Date.Add(this.SearchTime.Time);
            property.Hours = (int)this.HoursSlider.Value;

            var bookParking = new BookParking();
            bookParking.BindingContext = new BookParkingModel(property);
            await Navigation.PushAsync(bookParking);
        }

        async void Search_SuggestionItemSelected(object sender, Telerik.XamarinForms.Input.AutoComplete.SuggestionItemSelectedEventArgs e) {
            var address = e.DataItem.ToString();
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

		private void ParkNow_Clicked(object sender, EventArgs e) {
            SearchDateTime.IsVisible = false;
		}

		private void ParkLater_Clicked(object sender, EventArgs e) {
			SearchDateTime.IsVisible = true;
		}

        private async void FindParking_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Home());
        }

        private async void MyBookings_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Bookings());
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

            Hours.Text = string.Format("{0} hours", HoursSlider.Value);
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e) {
            LoadData();
        }

    }
}

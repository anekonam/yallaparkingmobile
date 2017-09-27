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
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class Home : ContentPage {
        private Xamarin.Forms.Maps.Position mapPosition;

        public Home() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Home Page");

            PropertyUtility.SetValue("LoggedIn", "true");

            UpdateCurrentLocation();

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;

            SearchDate.Date = DateTime.Now;

            Search.ItemsSource = new List<string>(){
              "Al Furjan",
              "Al Furjan Masakin",
              "Dubai Airport Free Zone",
              "Churchill Towers",
              "Clover Bay Tower 1",
              "The Citadel Tower",
              "Ontario Tower",
              "Prime Tower",
              "Executive Towers",
              "Oxford Tower",
              "The Bay Gate",
              "Ubora Towers",
              "Park Tower",
              "The Lofts",
              "Index Tower",
              "8 Boulevard Walk",
              "Claren Tower",
              "Central Park Towers",
              "Reehan 7",
              "Dorra Bay",
              "Marina Pinnacle",
              "DoubleTree by Hilton Hotel Dubai",
              "Sulafa Tower",
              "Al Sahab Tower 1",
              "Lago Vista Block C",
              "DoubleTree by Hilton Hotel Dubai",
              "Fortune Tower",
              "Global Lake View Tower",
              "Lake Terrace Tower",
              "Saba 3 Tower",
              "Jebel Ali Village",
              "Al Qusais",
              "Business Bay",
              "Trade Centre",
              "Downtown Dubai",
              "Dubai Marina",
              "Dubai Production City",
              "Jumeirah Beach Residences",
              "Jumeirah Lakes Towers"
            };            
            
            LoadData();            
        }

        async void LoadData() {
            BusyIndicator.IsBusy = true;
            await Task.Delay(2000);
            var profile = await ServiceUtility.Profile();
            BusyIndicator.IsBusy = false;
        }

        async void Search_SuggestionItemSelected(object sender, Telerik.XamarinForms.Input.AutoComplete.SuggestionItemSelectedEventArgs e) {
            LoadData();

            var address = e.DataItem.ToString();

            if (!string.IsNullOrWhiteSpace(address)) {
                address = string.Format("{0}, Dubai", address);
                var geocoder = new Geocoder();
                var positions = await geocoder.GetPositionsForAddressAsync(address);

                if (positions != null && positions.Any()) {
                    foreach (var position in positions) {
                        Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(1)));
                    }
                }
            }
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

        private async void FindParking_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Home());
        }

        private async void MyBookings_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Bookings());
        }

        private async void MyProfile_Clicked(object sender, EventArgs e) {
            await DisplayAlert("Profile Error", "Unable to retrieve your profile details, please check your internet connection and try again", "OK").ConfigureAwait(false);
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

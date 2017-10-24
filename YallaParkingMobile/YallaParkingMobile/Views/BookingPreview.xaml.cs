using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using System.Collections.ObjectModel;
using ZXing.Net.Mobile.Forms;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;


namespace YallaParkingMobile {
    public partial class BookingPreview : ContentPage {
        private IGeolocator locator = null;

		public BookingPreview() {
			InitializeComponent();
		}

		public BookingPreview(BookParkingModel model) {
			this.Model = model;

			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Page");

            this.UpdateCurrentLocation();
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

				locator = CrossGeolocator.Current;

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

		private void UpdateCurrentLocation() {
			this.GetCurrentLocation().ContinueWith(response => {
                this.Model.CurrentLocation = response.Result;
			});
		}

        public BookParkingModel Model {
            get {
                return (BookParkingModel)this.BindingContext;
            }
            set {
                this.BindingContext = value;
            }
        }

		async void Book_Clicked(object sender, System.EventArgs e) {			
			var bookParking = new BookParking(this.Model);
			await Navigation.PushAsync(bookParking);
		}
    }
}

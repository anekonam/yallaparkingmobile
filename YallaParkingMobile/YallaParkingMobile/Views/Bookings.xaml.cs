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
using YallaParkingMobile.Utility;
using ZXing.Net.Mobile.Forms;

namespace YallaParkingMobile {
    public partial class Bookings : ContentPage {        
        public Bookings() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Booking Page");          
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
            await Navigation.PushAsync(new Profile());
        }

        private async void Invite_Clicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new Invite());
        }

        private async void Logout_Clicked(object sender, EventArgs e) {
            PropertyUtility.RemoveKey("token");
            await Navigation.PushAsync(new Login());
        }

        private async void SearchButton_Clicked(object sender, EventArgs e) {
			var scanPage = new ZXingScannerPage();

			scanPage.OnScanResult += (result) => {
				// Stop scanning
				scanPage.IsScanning = false;

				// Pop the page and show the result
				Device.BeginInvokeOnMainThread(() => {
					Navigation.PopAsync();
					DisplayAlert("Scanned Barcode", result.Text, "OK");
				});
			};

			// Navigate to our scanner page
			await Navigation.PushAsync(scanPage);
        }
    }
}

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
    public partial class Profile : ContentPage {

        public Profile() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Profile Page");

            this.Appearing += Handle_Appearing;
        }

        async void Handle_Appearing(object sender, EventArgs e) {
            var profile = await ServiceUtility.Profile();
            this.ProfileName.Text = profile.Name;
        }

        void Handle_ContactTapped(object sender, System.EventArgs e) {
            Device.OpenUri(new Uri("tel:+971566595697"));
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
    }
}

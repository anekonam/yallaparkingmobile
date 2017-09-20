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

namespace YallaParkingMobile {
    public partial class Invite : ContentPage {        
        public Invite() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Invite Page");          
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
            await Navigation.PushAsync(new Login());
        }       
    }
}

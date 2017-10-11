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
using ZXing.Net.Mobile.Forms;

namespace YallaParkingMobile {
    public partial class BookingConfirmation : ContentPage {        
        public BookingConfirmation() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Booking Confirmation");          
        }       

       
        private async void DoneButton_Clicked(object sender, EventArgs e) {
			await Navigation.PushAsync(new Home());
        }
    }
}

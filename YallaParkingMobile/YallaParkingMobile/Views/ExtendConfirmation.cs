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
using YallaParkingMobile.Model;

namespace YallaParkingMobile {
    public partial class ExtendConfirmation : ContentPage {        
        public ExtendConfirmation() {
            InitializeComponent();        
        }

		public ExtendConfirmation(string extension) {
			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Confirmation");

            this.Instruction.Text = extension;
		}

		private async void DoneButton_Clicked(object sender, EventArgs e) {			
            await Navigation.PopAsync();
        }
    }
}

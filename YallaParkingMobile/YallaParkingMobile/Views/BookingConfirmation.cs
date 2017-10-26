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
using YallaParkingMobile.Model;

namespace YallaParkingMobile {
    public partial class BookingConfirmation : ContentPage {        
        public BookingConfirmation() {
            InitializeComponent();        
        }

		public BookingConfirmation(int entryMinutes) {
			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Confirmation");

            if (entryMinutes > 0) {
                this.Instruction.Text = string.Format("No need to rush, you can arrive {0} minutes before your bookings starts for free!", entryMinutes);
            } else{
                this.Instruction.Text = string.Format("Your parking space location is on the mezzanine floor");
            }
		}

        public BookingModel Model{
            get{
                return (BookingModel)this.BindingContext;
            } set{
                this.BindingContext = value;
            }
        }

		private async void DoneButton_Clicked(object sender, EventArgs e) {
			var model = new HomeModel();
			var home = new Home(model);

			await Navigation.PushAsync(home);
        }
    }
}

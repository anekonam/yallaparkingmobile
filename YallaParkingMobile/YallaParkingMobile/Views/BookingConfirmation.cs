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
using Plugin.SimpleAudioPlayer;

namespace YallaParkingMobile {
    public partial class BookingConfirmation : ContentPage {

		public BookingConfirmation(BookParkingModel model) {
            this.Model = model;
			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Confirmation");

			var player = CrossSimpleAudioPlayer.Current;
			player.Load("success.m4a");
			player.Play();

            if (model.BufferMinutes > 0 && !model.ParkNow) {
                this.Instruction.Text = string.Format("No need to rush, you can arrive {0} minutes before your bookings starts for free!", model.BufferMinutes);
                this.BookingReference.Text = model.BookingNumber;
            } else{
                this.Instruction.Text = string.Format("{0}. Parking Start time is {1}", model.AccessInfo, DateTime.UtcNow.ToLocalTime());
                this.BookingReference.Text = model.BookingNumber;
            }
		}

		public BookParkingModel Model {
			get {
				return (BookParkingModel)this.BindingContext;
			}
			set {
				this.BindingContext = value;
			}
		}


		private async void DoneButton_Clicked(object sender, EventArgs e) {
			var model = new BookingsModel();
			var booking = new Bookings(model);

			await Navigation.PushAsync(booking);
        }
    }
}

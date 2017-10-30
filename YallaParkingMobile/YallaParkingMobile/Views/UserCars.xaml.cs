using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using Plugin.Media.Abstractions;
using System.Collections.ObjectModel;

namespace YallaParkingMobile {
    public partial class UserCars : ContentPage {

        public UserCars(BookingModel model) {
            this.Booking = model;
            InitializeComponent();
            Analytics.TrackEvent("Viewing User Cars");
        }

        public GarageModel Model {
            get {
                return (GarageModel)this.BindingContext;
            }                
        }

        public BookingModel Booking { get; set; }

        async void Handle_Appearing(object sender, System.EventArgs e) {
            this.BusyIndicator.IsBusy = true;
            await LoadCars();
        }

		async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e) {

            var item = (UserCarModel)e.Item;

			bool confirm = await DisplayAlert("Change Car", "Are you sure you wish to change your existing car ?", "Yes", "No");

            if (confirm){

                this.Booking.UserCarId = item.UserCarId.Value;
                
                var result = await ServiceUtility.Update(this.Booking);
            }

			await Navigation.PopAsync();
		}

        async Task LoadCars(){
			var userCars = await ServiceUtility.GetUserCars();

			if (userCars != null && userCars.Any()) {
				this.Model.UserCars = new ObservableCollection<UserCarModel>(userCars);
			}

            this.BusyIndicator.IsBusy = false;
        }
    }
}

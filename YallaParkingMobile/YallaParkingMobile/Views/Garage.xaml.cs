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
    public partial class Garage : ContentPage {

        public Garage() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Garage Page");
        }

        public GarageModel GarageModel {
            get {
                return (GarageModel)this.BindingContext;
            }                
        }

        async void Handle_Appearing(object sender, System.EventArgs e) {
            this.BusyIndicator.IsBusy = true;
            await LoadCars();
        }

        async Task LoadCars(){
			var userCars = await ServiceUtility.GetUserCars();

			if (userCars != null && userCars.Any()) {
				this.GarageModel.UserCars = new ObservableCollection<UserCarModel>(userCars);
			}

            this.BusyIndicator.IsBusy = false;
        }

        async void AddNewButton_Clicked(object sender, EventArgs e) {
			var updateCarDetails = new UpdateCarDetails();
			var userCar = new UserCarModel();
			updateCarDetails.BindingContext = userCar;
			await Navigation.PushAsync(updateCarDetails);
		}

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e) {
			var updateCarDetails = new UpdateCarDetails();
            var userCar = e.Item as UserCarModel;
            updateCarDetails.BindingContext = userCar;
			await Navigation.PushAsync(updateCarDetails);
        }

        async void Delete_Clicked(object sender, System.EventArgs e) {
			var item = (MenuItem)sender;
            var userCar = item.CommandParameter as UserCarModel;

            if (userCar != null) {
                var result = await ServiceUtility.DeleteUserCar(userCar);

                if (!result) {
                    await DisplayAlert("Delete Car Error", "Unable to delete car", "Ok");
                } else{
                    this.GarageModel.UserCars.Remove(userCar);
                }
            }
        }
    }
}

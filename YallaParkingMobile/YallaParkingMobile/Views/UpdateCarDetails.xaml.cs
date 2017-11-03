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
using YallaParkingMobile.Control;

namespace YallaParkingMobile {
    public partial class UpdateCarDetails : ContentPage {

        public UpdateCarDetails() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Update Car Details Page");

            var picker = new BorderlessPicker {
                Title = "Select Make...",
                ItemsSource = new List<string>{
                    "Alfa Romeo",
                    "Aston Martin",
                    "Audi",
                    "BAIC Motor",
                    "Bentley",
                    "BMW",
                    "Brilliance",
                    "Bugatti",
                    "Buick",
                    "Cadillac",
                    "Caterham",
                    "Changan",
                    "Chery",
                    "Chevrolet",
                    "Chrysler",
                    "Citroen",
                    "Corvette",
                    "Daewoo",
                    "Daihatsu",
                    "Dodge",
                    "Ferrari",
                    "Fiat",
                    "Fisker",
                    "Ford",
                    "Foton",
                    "Geely",
                    "GMC",
                    "Honda",
                    "Hummer",
                    "Hyundai",
                    "Infiniti",
                    "Isuzu",
                    "Jaguar",
                    "Jeep",
                    "Jiangling",
                    "Kia",
                    "Koenigsegg",
                    "Lamborghini",
                    "Lancia",
                    "Land Rover",
                    "Lexus",
                    "Lincoln",
                    "Lotus",
                    "Maserati",
                    "Maybach",
                    "Mazda",
                    "McLaren",
                    "Mercedes Benz",
                    "Mercury",
                    "MG",
                    "Mini",
                    "Mitsubishi",
                    "Nissan",
                    "Opel",
                    "Peugeot",
                    "Pontiac",
                    "Porsche",
                    "Ram",
                    "Renault",
                    "Rolls Royce",
                    "Saab",
                    "Seat",
                    "Skoda",
                    "Smart",
                    "Spyker",
                    "SsangYong",
                    "Subaru",
                    "Suzuki",
                    "Tata",
                    "Tesla",
                    "Toyota",
                    "TVR",
                    "Volkswagen",
                    "Volvo"
                }
            };

            picker.SetBinding(Picker.SelectedItemProperty, "Make", BindingMode.TwoWay);
            this.Make.Picker = picker;

            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        public UserCarModel Model {
            get {
                return (UserCarModel)this.BindingContext;
            }                
        }   

        public BookParkingModel BookParking { get; set; }

        async void UpdateButton_Clicked(object sender, EventArgs e) {
            if(string.IsNullOrWhiteSpace(this.Model.Make)){
                await DisplayAlert("Make Required", "Please select a car Make", "Ok");
                return;
            } else if(string.IsNullOrWhiteSpace((this.Model.ModelNumber))){
                await DisplayAlert("Model Required", "Please provide a car Model", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace((this.Model.RegistrationNumber))) {
				await DisplayAlert("Registration Required", "Please provide a car Registration", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace((this.Model.Color))) {
				await DisplayAlert("Colour Required", "Please provide a car Colour", "Ok");
                return;
			}
               
            Activity.IsVisible = true;
            Activity.IsRunning = true;

			var userCar = await ServiceUtility.UpdateUserCar(this.Model);

			Activity.IsRunning = false;
			Activity.IsVisible = false;

			if (userCar == null) {
				await DisplayAlert("Car Save Error", "Unable to save your car details, please ensure your card details are valid", "Ok");
			} else {
				var bookParking = new BookParking(this.BookParking);
				await Navigation.PushAsync(bookParking);
				await this.Navigation.PopAsync();
			}
        }
    }
}

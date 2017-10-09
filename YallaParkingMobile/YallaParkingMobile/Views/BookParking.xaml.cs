using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using System.Collections.ObjectModel;

namespace YallaParkingMobile {
    public partial class BookParking : ContentPage {
        public BookParking() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Booking Page");

            Appearing += BookParking_Appearing;                     
        }

        public BookParkingModel Model{
            get{
                return (BookParkingModel)this.BindingContext;
            }
        }

        async void BookParking_Appearing(object sender, EventArgs e) {            
            if (!string.IsNullOrWhiteSpace(this.Model.Property.ImageBase)) {
                this.PropertyImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(this.Model.Property.ImageBase)));
            }

            NavigationPage.SetBackButtonTitle(this, " ");

            var userCars = await ServiceUtility.GetUserCars();

			if (userCars != null && userCars.Any()) {
                this.Model.UserCars = new ObservableCollection<UserCarModel>(userCars);
			}
        }

        private async void ApplyCodeButton_Clicked(object sender, EventArgs e) {
            var discountCode = DiscountCode.Text;

            if (!string.IsNullOrWhiteSpace(discountCode)) {
                var discountResponse = await ServiceUtility.DiscountCode(discountCode);

                if (discountResponse.Discount.HasValue) {
                    this.Model.Property.Discount = this.Model.Property.TotalPrice * (discountResponse.Discount.Value / 100);
                    this.ApplyCodeButton.IsVisible = false;
                    this.DiscountCode.IsEnabled = false;
                    Order.Remove(DiscountCode);
                    Order.Remove(ApplyCodeCell);
                    await DisplayAlert("Promotional Code Validated", "Your promotional code has been successfully validated.", "Ok");                   
                } else {
                    await DisplayAlert("Invalid Promotional Code", "Invalid or expired promotional code provided.", "Ok");
                }
            } else {
                await DisplayAlert("No Promotional Code", "No promotional code provided.", "Ok");
            }
        }

		async void AddNewButton_Clicked(object sender, EventArgs e) {
			var updateCarDetails = new UpdateCarDetails();
			var userCar = new UserCarModel();
			updateCarDetails.BindingContext = userCar;
			await Navigation.PushAsync(updateCarDetails);
		}

		async void Park_Clicked(object sender, System.EventArgs e) {
            await Navigation.PopAsync();
		}

        async void Book_Clicked(object sender, System.EventArgs e) {
            await Navigation.PopAsync();
        }
    }
}

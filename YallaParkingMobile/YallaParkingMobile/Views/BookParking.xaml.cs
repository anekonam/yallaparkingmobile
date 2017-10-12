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
        }

        public BookParking(BookParkingModel model) {
            this.Model = model;

			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Page");

			Appearing += BookParking_Appearing;
		}

        public BookParkingModel Model{
            get{
                return (BookParkingModel)this.BindingContext;
            } set{
                this.BindingContext = value;
            }
        }

        async void BookParking_Appearing(object sender, EventArgs e) {            
            if (!string.IsNullOrWhiteSpace(this.Model.Property.ImageBase)) {
                this.PropertyImage.Source = ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/image/" + Model.Property.PropertyId));
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
                    this.Model.Discount = discountResponse.Discount.Value;
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

        async void Book_Clicked(object sender, System.EventArgs e) {
            var booking = await Model.BookParking();

            if (!booking) {
                await DisplayAlert("Booking Error", "There was an error confirming your booking, please try again", "Ok");
            } else {
                var bookingConfirmation = new BookingConfirmation();
                bookingConfirmation.BindingContext = Model.BookingNumber;
                await Navigation.PushAsync(bookingConfirmation);
            }
        }
    }
}

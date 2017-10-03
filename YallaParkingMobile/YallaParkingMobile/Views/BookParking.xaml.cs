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

namespace YallaParkingMobile {
    public partial class BookParking : ContentPage {
        public BookParking() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Booking Page");

            Appearing += BookParking_Appearing;                     
        }

        private void BookParking_Appearing(object sender, EventArgs e) {
            var property = (PropertyModel)this.BindingContext;

            if (!string.IsNullOrWhiteSpace(property.ImageBase)) {
                this.PropertyImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(property.ImageBase)));
            }

            NavigationPage.SetBackButtonTitle(this, " ");
            this.VehicleSelect.ItemsSource = new string[] { "Car 1", "Car 2", "Car 3"};
        }

        private async void ApplyCodeButton_Clicked(object sender, EventArgs e) {
            var discountCode = DiscountCode.Text;

            if (!string.IsNullOrWhiteSpace(discountCode)) {
                var discountResponse = await ServiceUtility.DiscountCode(discountCode);

                if (discountResponse.Discount.HasValue) {
                    var property = (PropertyModel)this.BindingContext;
                    property.Discount = property.TotalPrice * (discountResponse.Discount.Value / 100);
                    this.ApplyCodeButton.IsVisible = false;
                    this.DiscountCode.IsEnabled = false;
                    this.ApplyCodeCell.Height = 0;
                    await DisplayAlert("Promotional Code Validated", "Your promotional code has been successfully validated.", "Ok");                   
                } else {
                    await DisplayAlert("Invalid Promotional Code", "Invalid or expired promotional code provided.", "Ok");
                }
            } else {
                await DisplayAlert("No Promotional Code", "No promotional code provided.", "Ok");
            }
        }
    }
}

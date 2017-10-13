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
using ZXing.Net.Mobile.Forms;

namespace YallaParkingMobile {
    public partial class BookingDetail : ContentPage {
        public BookingDetail() {
            InitializeComponent();             
        }

        public BookingDetail(BookingModel model) {
            this.Model = model;

			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Page - "+Model.Number);
		}

        public BookingModel Model {
            get {
                return (BookingModel)this.BindingContext;
            }
            set {
                this.BindingContext = value;
            }
        }

		async void Handle_Appearing(object sender, System.EventArgs e) {
            await this.RefreshBooking();
		}

        public async Task RefreshBooking(){
            this.Model = await ServiceUtility.GetBooking(this.Model.PropertyParkingId);
        }

		private async void ScanEntry_Clicked(object sender, EventArgs e) {
			var scanPage = new ZXingScannerPage();
			bool scanFinished = false;

			scanPage.OnScanResult += (result) => {
				// Stop scanning
				scanPage.IsScanning = false;

				// Pop the page and show the result
				Device.BeginInvokeOnMainThread(async () => {
					if (!scanFinished) {
						scanFinished = true;

                        if(result.Text == Model.PropertyId.ToString()){
                            await ServiceUtility.Entry(Model.PropertyId);
							await this.RefreshBooking();
                            await DisplayAlert("Valid Scan", "Your scan has been validated for entry to your parking space", "Ok");
                            await Navigation.PopAsync();
                        } else{
                            await DisplayAlert("Invalid Scan", "The QR code scanned does not match the property for this booking", "Ok");
                            await Navigation.PopAsync();
                        }
					}
				});
			};

			// Navigate to our scanner page
			await Navigation.PushAsync(scanPage);
		}

		private async void ScanExit_Clicked(object sender, EventArgs e) {
			var scanPage = new ZXingScannerPage();
			bool scanFinished = false;

			scanPage.OnScanResult += (result) => {
				// Stop scanning
				scanPage.IsScanning = false;

				// Pop the page and show the result
				Device.BeginInvokeOnMainThread(async () => {
					if (!scanFinished) {
						scanFinished = true;

						if (result.Text == Model.PropertyId.ToString()) {
							await ServiceUtility.Exit(Model.PropertyId);
                            await this.RefreshBooking();
							await DisplayAlert("Valid Scan", "Your scan has been validated for exit from your parking space", "Ok");
                            await Navigation.PopAsync();
                            await Navigation.PushAsync(new ConfirmExit());
						} else {
							await DisplayAlert("Invalid Scan", "The QR code scanned does not match the property for this booking", "Ok");
							await Navigation.PopAsync();
						}
					}
				});
			};

			// Navigate to our scanner page
			await Navigation.PushAsync(scanPage);
		}
    }
}

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

            if (this.Model.Discounted || !this.Model.Active) {
				if (this.TableView.Contains(this.ValidateParking)) {
					this.TableView.Remove(this.ValidateParking);
				}
            } else{
				if (!this.TableView.Contains(this.ValidateParking)) {
                    this.TableView.Insert(1, this.ValidateParking);
				}
            }
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
                            var entry = await ServiceUtility.Entry(Model.PropertyId);

                            if (entry) {
                                await this.RefreshBooking();
                                await DisplayAlert("Valid Scan", "Your scan has been validated for entry to your parking space", "Ok");
                                await Navigation.PopAsync();
                            } else{
								await DisplayAlert("Entry Error", "There was an error entering the parking space, please try again", "Ok");
                            }
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
							var exit = await ServiceUtility.Exit(Model.PropertyId);

                            if (exit) {
                                await this.RefreshBooking();
                                await Navigation.PushAsync(new ConfirmExit());
                            } else{
                                await DisplayAlert("Exit Error", "There was an error leaving the parking space, please try again", "Ok");
                            }
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

        private async void Validate_Clicked(object sender, System.EventArgs e) {
			var scanPage = new ZXingScannerPage();
			bool scanFinished = false;

			scanPage.OnScanResult += (result) => {
				// Stop scanning
				scanPage.IsScanning = false;

				// Pop the page and show the result
				Device.BeginInvokeOnMainThread(async () => {
					if (!scanFinished) {
						scanFinished = true;

                        var validatorUserId = Model.ValidatorUserIds.Any(u => u.ToString() == result.Text) ?
                                                   Model.ValidatorUserIds.First(u => u.ToString() == result.Text) : (int?)null;                                  

                        if (validatorUserId.HasValue) {
                            var validated = await ServiceUtility.Validate(Model.PropertyParkingId, validatorUserId.Value);

                            if (validated) {
                                await this.RefreshBooking();
                                await Navigation.PopAsync();
                            } else{
								await DisplayAlert("Validate Error", "There was an error validating your parking, please try again", "Ok");
                            }
						} else {
							await DisplayAlert("Invalid Scan", "The QR code scanned does not match a validator for this booking", "Ok");
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

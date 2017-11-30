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
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using Plugin.MediaManager;
using Plugin.SimpleAudioPlayer;

namespace YallaParkingMobile {
    public partial class BookingDetail : ContentPage {
        private IGeolocator locator = null;

        private EntryCell cancellationCharge;

        public BookingDetail() {
            InitializeComponent();             
        }

        public BookingDetail(BookingModel model) {
            this.Model = model;

			InitializeComponent();
			Analytics.TrackEvent("Viewing Booking Page - " + Model.Number);

			if (this.Model.Hours >= 8) {
                    Order.Remove(ParkNowPriceDay);
			} else {
				if (Order.Contains(PriceDay)) {
					Order.Remove(PriceDay);
				}
			}

            if(this.Model.AllDay || (this.Model.ParkLater && this.Model.Hours >= 8)){
                Order.Remove(PriceHour);
                Order.Remove(ParkNow);
                Order.Remove(ParkLater);
            }

            if(!this.Model.AllDay || (this.Model.ParkLater && this.Model.Hours < 8)){
                Order.Remove(TotalDays);
            }

            if(this.Model.ParkNow && !this.Model.AllDay){
                Order.Remove(ParkNowPriceDay);
                Order.Remove(PriceDay);
            }

			if (!this.Model.ParkNow && !this.Model.AllDay) {
				Order.Remove(ParkNowPriceDay);
			}

            if(this.Model.ParkNow && this.Model.AllDay){
                Order.Remove(PriceHour);
				Order.Remove(PriceDay);
			}

            if(this.Model.ParkLater){
                Order.Remove(ParkNow);
                Order.Remove(ParkNowDiscount);
            } else{
                Order.Remove(ParkLaterDiscount);
                Order.Remove(ParkLater);
            }

			if (this.Model.ParkNow && this.Model.AllDay) {
				Order.Remove(ParkLater);
                Order.Remove(ParkLaterDiscount);
			}

            this.cancellationCharge = CancellationCharge;

            Order.Remove(CancellationCharge);


            if(!this.Model.CancellationMessage){
                TableView.Remove(CancellationPolicy);
            }
		}
             
		private async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentLocation() {
			Plugin.Geolocator.Abstractions.Position position = null;

			try {
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location)) {
					await DisplayAlert("Location Required", "In order to use this application you must be able to share your location.", "OK");
				}

				var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
				//Best practice to always check that the key exists
				if (results.ContainsKey(Permission.Location)) {
					status = results[Permission.Location];
				}

				locator = CrossGeolocator.Current;

				if (status == PermissionStatus.Granted) {
					locator.DesiredAccuracy = 100;

					position = await locator.GetLastKnownLocationAsync();

					if (position != null) {
						return position;
					}

					if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled) {
						return null;
					}

					position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
				}
			} catch (Exception ex) {
				Debug.WriteLine(ex);
			}

			if (position == null) {
				return position;
			}

			var output = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
				position.Timestamp, position.Latitude, position.Longitude,
				position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

			Analytics.TrackEvent(output);

			return position;
		}

		private void UpdateCurrentLocation() {
			this.GetCurrentLocation().ContinueWith(response => {
                this.Model.CurrentLocation = response.Result;
			});
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

			

			UpdateCurrentLocation();
		}        	


		public async Task RefreshBooking(){
            this.Model = await ServiceUtility.GetBooking(this.Model.PropertyParkingId);

            if (this.Model.Discounted || (!this.Model.Active && !this.Model.Completed)) {
				if (this.TableView.Contains(this.ValidateParking)) {
					this.TableView.Remove(this.ValidateParking);
				}
            } else if(!this.Model.ValidatorUserId.HasValue && this.Model.Completed){
                this.TableView.Remove(this.ValidateParking);
                
            } else{
				if (!this.TableView.Contains(this.ValidateParking)) {
                    this.TableView.Insert(1, this.ValidateParking);
				}
            }
			if (this.Model.Cancelled.HasValue) {
				Order.Add(cancellationCharge);
			}
        }

		private async void ScanEntry_Clicked(object sender, EventArgs e) {
            var timeToBooking = (this.Model.Start - DateTime.UtcNow).TotalMinutes;

            if (timeToBooking > 30) {
                await DisplayAlert("You're Early", "Hey eager beaver, you're early. You can only scan in within 30 minutes of a booking. You will start being charged from the time you scan in", "Ok");
            } else {

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
						
                                var entry = await ServiceUtility.Entry(Model.PropertyId);

                                if (entry) {
                                    await this.RefreshBooking();
                                    await Navigation.PopAsync();
                                } else {
                                    await DisplayAlert("Entry Error", "There was an error entering the parking space, please try again", "Ok");
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
								this.Model.ExitTime = DateTime.UtcNow;
								await this.RefreshBooking();
								await Navigation.PushAsync(new ConfirmExit(this.Model));
							} else {
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

		private async void Submit_Clicked(object sender, EventArgs e) {

            bool confirm = await DisplayAlert("Extend Parking", "Are you sure you wish to extend your parking by " + this.Model.ExtensionHours + " " + (this.Model.ExtensionHours == 1 ? "hour" : "hours") +"?", "Yes", "No");

			if (confirm) {		    
                this.Model.End = this.Model.End.AddHours(this.Model.ExtensionHours);
			    var result = await ServiceUtility.Update(this.Model);

			    if (result) {
                    var extension = string.Format("{0} {1} from {2:h:mm tt} to {3:h:mm tt}", this.Model.ExtensionHours, this.Model.ExtensionHours == 1 ? "hour" : "hours", this.Model.StartLocal, this.Model.EndLocal);
                   
			        await Navigation.PushAsync(new ExtendConfirmation(extension));
			    }
			} 

		}

        private void Plus_Clicked(object sender, EventArgs e){
            var extendedHours = this.Model.Hours + this.Model.ExtensionHours;
            var maxHours = (int)Math.Round((DateTime.UtcNow.AddDays(1).Date - DateTime.UtcNow).TotalHours, 0);
            maxHours = maxHours <= 7 ? maxHours : 7;

            if (extendedHours < maxHours) {
                this.Model.ExtensionHours++;
            }
	    }

		private void Minus_Clicked(object sender, EventArgs e) {
            if (this.Model.ExtensionHours > 0) {
                this.Model.ExtensionHours--;
            }
		}

		private async void Cancel_Clicked(object sender, EventArgs e) {
            var confirm = await DisplayAlert("Cancel Booking", "Are you sure you wish to cancel your booking? Please note this may incur a late cancellation charge", "Yes", "No");

            if (confirm){
                var result = await ServiceUtility.Cancel(this.Model.PropertyParkingId);

                if(result){
                    await this.RefreshBooking();
                    await DisplayAlert("Booking Cancelled", "Your booking has been successfully cancelled", "Ok");                    
                } else{
					await DisplayAlert("Booking Cancellation Error", "There was a problem cancelling your booking, please try again", "Ok");
				}
            }
		}

		async void Car_Tapped(object sender, EventArgs e) {
			var car = new UserCars(this.Model);
			car.BindingContext = new GarageModel();
			await Navigation.PushAsync(car);
		}

		async void Card_Tapped(object sender, EventArgs e) {
			var card = new UserCards(this.Model);
			card.BindingContext = new WalletModel();
			await Navigation.PushAsync(card);
		}
    }
}

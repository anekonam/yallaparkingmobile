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
	public partial class BookParking : ContentPage {

        private TableSection oneCar;
        private TableSection twoCars;

		private TableSection oneCard;
		private TableSection twoCards;

        void Handle_Appearing(object sender, System.EventArgs e) {
            throw new NotImplementedException();
        }

        public BookParking() {
			InitializeComponent();           
		}

		public BookParking(BookParkingModel model) {
			this.Model = model;

			InitializeComponent();
			
            this.oneCar = OneCar;
			this.twoCars = TwoCars;

			this.oneCard = OneCard;
			this.twoCards = TwoCards;

			Analytics.TrackEvent("Viewing Booking Page");

			Appearing += BookParking_Appearing;

			if (this.Model.AllDay) {
				if (Order.Contains(PriceHour)) {
					Order.Remove(PriceHour);
				}
			} else {
				if (Order.Contains(PriceDay)) {
					Order.Remove(PriceDay);
				}
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

		async void BookParking_Appearing(object sender, EventArgs e) {
			NavigationPage.SetBackButtonTitle(this, " ");

			var userCars = await ServiceUtility.GetUserCars();

			if (userCars != null && userCars.Any()) {
				this.Model.UserCars = new ObservableCollection<UserCarModel>(userCars);
				if (this.Model.UserCars.Count == 1) {
					this.Model.TotalCar = 1;
				} else if (this.Model.UserCars.Count == 2) {
					this.Model.TotalCar = 2;
				} else {
					this.Model.TotalCar = this.Model.UserCars.Count();
				}
			}

			TableView.Remove(TwoCars);
			TableView.Remove(OneCar);

			if (this.Model.TotalCar == 1) {
                TableView.Add(oneCar);
			} else if (this.Model.TotalCar >= 2) {
                TableView.Add(twoCars);
			} 

			var userCards = await ServiceUtility.GetUserCards();

			if (userCards != null && userCards.Any()) {
				this.Model.UserCards = new ObservableCollection<UserCardModel>(userCards);
				if (this.Model.UserCards.Count == 1) {
					this.Model.TotalCard = 1;
				} else if (this.Model.UserCards.Count == 2) {
					this.Model.TotalCard = 2;
				} else {
					this.Model.TotalCard = this.Model.UserCards.Count();
				}
			}

			TableView.Remove(TwoCards);
			TableView.Remove(OneCard);

			if (this.Model.TotalCard == 1) {
				TableView.Add(oneCard);
			} else if (this.Model.TotalCard >= 2) {
				TableView.Add(twoCards);
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

		async void AddNewCarButton_Clicked(object sender, EventArgs e) {
			var updateCarDetails = new UpdateCarDetails();
			var userCar = new UserCarModel();
			updateCarDetails.BindingContext = userCar;
			await Navigation.PushAsync(updateCarDetails);
		}

		async void AddNewCardButton_Clicked(object sender, EventArgs e) {
			var updateCardDetails = new UpdateCardDetails();
			var userCard = new UserCardModel();
			updateCardDetails.BindingContext = userCard;
			await Navigation.PushAsync(updateCardDetails);
		}

		async void Book_Clicked(object sender, System.EventArgs e) {
			if (Model.ParkingNow) {
				var scanPage = new ZXingScannerPage();
				bool scanFinished = false;

				scanPage.OnScanResult += (result) => {
					// Stop scanning
					scanPage.IsScanning = false;

					// Pop the page and show the result
					Device.BeginInvokeOnMainThread(async () => {
						if (!scanFinished) {
							scanFinished = true;

							if (result.Text == Model.Property.PropertyId.ToString()) {
								var booking = await Model.BookParking();

								if (booking) {
									var entry = await ServiceUtility.Entry(Model.Property.PropertyId);

									if (entry) {
										await DisplayAlert("Valid Scan", "Your scan has been validated for entry to your parking space", "Ok");

										var bookingConfirmation = new BookingConfirmation(this.Model);
										bookingConfirmation.BindingContext = Model.BookingNumber;
										await Navigation.PushAsync(bookingConfirmation);
									} else {
										await DisplayAlert("Entry Error", "There was an error entering the parking space, please try again", "Ok");
									}
								} else {
									await DisplayAlert("Booking Error", "There was an error confirming your booking, please try again", "Ok");
									await Navigation.PopAsync();
								}

							} else {
								await DisplayAlert("Invalid Scan", "The QR code scanned does not match the property for this booking", "Ok");
								await Navigation.PopAsync();
							}
						}
					});
				};

				await Navigation.PushAsync(scanPage);
			} else {
				var booking = await Model.BookParking();

				if (!booking) {
					await DisplayAlert("Booking Error", "There was an error confirming your booking, please try again", "Ok");
				} else {
					var bookingConfirmation = new BookingConfirmation(this.Model);
					bookingConfirmation.BindingContext = Model.BookingNumber;
					await Navigation.PushAsync(bookingConfirmation);
				}
			}
		}
	}
}

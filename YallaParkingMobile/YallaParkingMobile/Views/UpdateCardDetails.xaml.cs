﻿using Microsoft.Azure.Mobile.Analytics;
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
using System.Net;

namespace YallaParkingMobile {
    public partial class UpdateCardDetails : ContentPage {

        public UpdateCardDetails() {
            InitializeComponent();

            Analytics.TrackEvent("Viewing Update Card Details Page");
         
            var months = new List<string>();

            for (var i = 0; i < 12;i++){
                months.Add((i + 1).ToString());
            }

			var monthPicker = new BorderlessPicker {
                Title = "Select Month...",
				ItemsSource = months
			};

			monthPicker.SetBinding(Picker.SelectedItemProperty, "ExpireMonth", BindingMode.TwoWay);
            this.ExpireMonth.Picker = monthPicker;

			var years = new List<string>();

			for (var i = 0; i < 10; i++) {
                years.Add(DateTime.Now.AddYears(i).Year.ToString());
			}

			var yearPicker = new BorderlessPicker {
                Title = "Select Year...",
				ItemsSource = years
			};

			yearPicker.SetBinding(Picker.SelectedItemProperty, "ExpireYear", BindingMode.TwoWay);
			this.ExpireYear.Picker = yearPicker;

            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        public UserCardModel Model {
            get {
                return (UserCardModel)this.BindingContext;
            }                
        }

        void Handle_Appearing(object sender, System.EventArgs e) {
			if (!this.Model.IsNew) {
				if (CardDetails.Contains(CardNumber)) {
					CardDetails.Remove(CardNumber);
				}

				if (CardDetails.Contains(Cvc)) {
					CardDetails.Remove(Cvc);
				}
			} else {
				if (CardDetails.Contains(EncodedCardNumber)) {
					CardDetails.Remove(EncodedCardNumber);
				}

				this.CardNumber.Placeholder = "•••• •••• •••• ••••";
				this.Cvc.Placeholder = "•••";
			}
		}
		

		private async void DeleteButton_Clicked(object sender, EventArgs e) {
			var confirm = await DisplayAlert("Delete Card", "Are you sure you wish to delete this card?", "Yes", "No");

			if (confirm) {
				var result = await ServiceUtility.DeleteUserCard(this.Model);

                if (result.IsSuccessStatusCode) {
					await DisplayAlert("Card Deleted", "Your card has been successfully deleted", "Ok");
					var card = new UserCards(new BookingModel());
					card.BindingContext = new WalletModel();
					await Navigation.PushAsync(card);
				} else {
                    if (result.StatusCode == HttpStatusCode.Forbidden) {
						await DisplayAlert("Card Used", "This card is associated with a booking and cannot be deleted", "Ok");
                    } else {
                        await DisplayAlert("Card Delete Error", "There was a problem deleting your card", "Ok");
                    }
				}
			}
		}

        async void UpdateButton_Clicked(object sender, EventArgs e) {
            SaveButton.IsEnabled = false;

			if(string.IsNullOrWhiteSpace((this.Model.Name))){
                await DisplayAlert("Name Required", "Please provide a Name", "Ok");
                SaveButton.IsEnabled = true;
                return;
            } else if (string.IsNullOrWhiteSpace((this.Model.ExpireMonth))) {
				await DisplayAlert("Expiry Month Required", "Please provide a card expiry month", "Ok");
                SaveButton.IsEnabled = true;
                return;
            } else if (string.IsNullOrWhiteSpace((this.Model.ExpireYear))) {
				await DisplayAlert("Expiry Year Required", "Please provide a card expiry year", "Ok");
                SaveButton.IsEnabled = true;
				return;
            } else if (string.IsNullOrWhiteSpace((this.Model.Number))) {
				await DisplayAlert("Card Number Required", "Please provide a card number", "Ok");
                SaveButton.IsEnabled = true;
                return;
			} else if (string.IsNullOrWhiteSpace((this.Model.Cvc))) {
				await DisplayAlert("CVC Required", "Please provide a CVC number", "Ok");
                SaveButton.IsEnabled = true;
				return;
			}
             
            Activity.IsVisible = true;
            Activity.IsRunning = true;

            var userCard = await ServiceUtility.UpdateUserCard(this.Model);

            SaveButton.IsEnabled = true;

			Activity.IsRunning = false;
			Activity.IsVisible = false;

            if (userCard == null) {
                await DisplayAlert("Card Save Error", "Unable to save your card details, please ensure your card details are valid", "Ok");
            } else {
                await DisplayAlert("Card Save Confirm", "Your card details have been successfully added to your wallet.", "Ok");
                await this.Navigation.PopAsync();
            }
        }
    }
}

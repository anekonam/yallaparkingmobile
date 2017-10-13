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
    public partial class Wallet : ContentPage {

        public Wallet() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Wallet Page");
        }

        public WalletModel Model {
            get {
                return (WalletModel)this.BindingContext;
            }                
        }

        async void Handle_Appearing(object sender, System.EventArgs e) {
            this.BusyIndicator.IsBusy = true;
            await LoadCards();
        }

        async Task LoadCards(){
			var userCards = await ServiceUtility.GetUserCards();

			if (userCards != null && userCards.Any()) {
				this.Model.UserCards = new ObservableCollection<UserCardModel>(userCards);
			}

            this.BusyIndicator.IsBusy = false;
        }

        async void AddNewButton_Clicked(object sender, EventArgs e) {
			var updateCardDetails = new UpdateCardDetails();
			var userCard = new UserCardModel();
			updateCardDetails.BindingContext = userCard;
			await Navigation.PushAsync(updateCardDetails);
		}

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e) {
			var updateCardDetails = new UpdateCardDetails();
            var userCard = e.Item as UserCardModel;
            updateCardDetails.BindingContext = userCard;
			await Navigation.PushAsync(updateCardDetails);
        }

        async void Delete_Clicked(object sender, System.EventArgs e) {
			var item = (MenuItem)sender;
            var userCard = item.CommandParameter as UserCardModel;

            if (userCard != null) {
                var result = await ServiceUtility.DeleteUserCard(userCard);

                if (!result) {
                    await DisplayAlert("Delete Card Error", "Unable to delete card", "Ok");
                } else{
                    this.Model.UserCards.Remove(userCard);
                }
            }
        }
    }
}

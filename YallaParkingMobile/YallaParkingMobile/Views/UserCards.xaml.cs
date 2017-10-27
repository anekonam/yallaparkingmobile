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
    public partial class UserCards : ContentPage {

        public UserCards() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing User Cards Page");
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

        public BookingModel Booking { get; set; }

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e) {
            
			var item = (UserCardModel)e.Item;

			bool confirm = await DisplayAlert("Change Card", "Are you sure you wish to change your existing card ?", "Yes", "No");

			if (confirm) {

				this.Booking.UserCardId = item.UserCardId.Value;

				var result = await ServiceUtility.Update(this.Booking);
			}

			await Navigation.PopAsync();
        }
    }
}

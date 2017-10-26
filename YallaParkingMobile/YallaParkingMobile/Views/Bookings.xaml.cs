using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YallaParkingMobile.Utility;
using YallaParkingMobile.Model;
using System.IO;

namespace YallaParkingMobile {
    public partial class Bookings : ContentPage {        
        public Bookings() {
            InitializeComponent();         
        }

        public Bookings(BookingsModel model) {
            this.Model = model;

			InitializeComponent();

			Analytics.TrackEvent("Viewing Booking Page");
		}

        public BookingsModel Model{
            get{
                return (BookingsModel)this.BindingContext;
            } set{
                this.BindingContext = value;
            }
        }

        public ProfileModel Profile { get; set; }

        async void Handle_Appearing(object sender, System.EventArgs e) {
            await this.Model.GetBookings();

			var profile = await ServiceUtility.Profile();
            this.Profile = profile;
			this.ProfileName.Text = profile.Name;
			if (!string.IsNullOrWhiteSpace(profile.ProfilePicture)) {
				var profileImage = !string.IsNullOrWhiteSpace(profile.ProfilePicture) && profile.ProfilePicture.Contains(",") ? profile.ProfilePicture.Split(',')[1] : profile.ProfilePicture;
				this.ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
			}
        }

        async void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e) {
            var model = (BookingModel)e.SelectedItem;
            var bookingDetail = new BookingDetail(model);
            await Navigation.PushAsync(bookingDetail);
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Menu.IsOpen = !Menu.IsOpen;
        }

		private async void SearchButton_Clicked(object sender, EventArgs e) {
			var model = new HomeModel();
			var home = new Home(model);

			await Navigation.PushAsync(home);
		}

        private async void FindParking_Clicked(object sender, EventArgs e) {
            var model = new HomeModel();
			var home = new Home(model);
			
			await Navigation.PushAsync(home);
        }

        private async void MyBookings_Clicked(object sender, EventArgs e) {
			var model = new BookingsModel();
			var bookings = new Bookings(model);

			await Navigation.PushAsync(bookings);
        }

        private async void MyProfile_Clicked(object sender, EventArgs e) {            
            await Navigation.PushAsync(new Profile());
        }

        private async void Invite_Clicked(object sender, EventArgs e) {
			var inviteCode = new Invite();
            inviteCode.BindingContext = this.Profile;
			await Navigation.PushAsync(inviteCode);
        }

    }
}

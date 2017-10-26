using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Plugin.Geolocator;
using Plugin.Share;
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
using System.Security.Principal;
using Plugin.Share.Abstractions;

namespace YallaParkingMobile {
    public partial class Invite : ContentPage {        
        public Invite() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Invite Page");
        }

		public ProfileModel Model {
			get {
				return (ProfileModel)this.BindingContext;
			}
		}

        private void Button_Clicked(object sender, EventArgs e) {
            Menu.IsOpen = !Menu.IsOpen;
        }

        async void Handle_Appearing(object sender, System.EventArgs e) {
			var profile = await ServiceUtility.Profile();
			this.ProfileName.Text = profile.Name;
			if (!string.IsNullOrWhiteSpace(profile.ProfilePicture)) {
				var profileImage = !string.IsNullOrWhiteSpace(profile.ProfilePicture) && profile.ProfilePicture.Contains(",") ? profile.ProfilePicture.Split(',')[1] : profile.ProfilePicture;
				this.ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
			}
        }

		private async void InviteNow_Clicked(object sender, EventArgs e) {

            if (!CrossShare.IsSupported) {
                return;
            }

               await CrossShare.Current.Share(new ShareMessage {
                    Title = "Test",
                    Text =string.Format("Discount Code - {0}",Model.InviteCode),
                    Url="www.insiso.co.uk"
                });

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
			inviteCode.BindingContext = this.Model;
			await Navigation.PushAsync(inviteCode);
        }
    }
}

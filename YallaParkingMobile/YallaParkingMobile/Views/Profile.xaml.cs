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

namespace YallaParkingMobile {
    public partial class Profile : ContentPage {

        public Profile() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Profile Page");

            this.Appearing += Handle_Appearing;
        }

        public ProfileModel Model {
            get {
                return (ProfileModel)this.BindingContext;
            }
        }

        async void Handle_Appearing(object sender, EventArgs e) {
            await LoadProfile();
        }

        async Task LoadProfile(){
            if (this.Model == null) {
                this.BusyIndicator.IsBusy = true;
            }

			var profile = await ServiceUtility.Profile();

			this.BindingContext = profile;

            if (profile.Verified){
                if (this.ProfileSection.Contains(VerifyProfile)) {
                    this.ProfileSection.Remove(VerifyProfile);
                }
            }

			if (!string.IsNullOrWhiteSpace(profile.ProfilePicture)) {
				var profileImage = !string.IsNullOrWhiteSpace(this.Model.ProfilePicture) && this.Model.ProfilePicture.Contains(",") ? this.Model.ProfilePicture.Split(',')[1] : this.Model.ProfilePicture;
				this.ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
                this.ProfileMenuImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
			}

            this.BusyIndicator.IsBusy = false;
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Menu.IsOpen = !Menu.IsOpen;
        }

        private async void FindParking_Clicked(object sender, EventArgs e) {
			var home = new Home(new HomeModel());
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
            await Navigation.PushAsync(new Invite());
        }

        async void ProfileImage_Tapped(object sender, EventArgs e) {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                PhotoSize = PhotoSize.Small,
                SaveToAlbum = false
            });

            if (file == null) {
                return;
            }

            using (MemoryStream stream = new MemoryStream()) {
                file.GetStream().CopyTo(stream);

                if (this.Model != null) {
                    this.Model.ProfilePicture = Convert.ToBase64String(stream.ToArray());
                    var profileImage = !string.IsNullOrWhiteSpace(this.Model.ProfilePicture) && this.Model.ProfilePicture.Contains(",") ? this.Model.ProfilePicture.Split(',')[1] : this.Model.ProfilePicture;
                    this.ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(profileImage)));
                }
            }

            await ServiceUtility.UpdateProfile(this.Model);
        }

        async void UpdateProfile_Tapped(object sender, EventArgs e) {
            var updateProfile = new UpdateProfile();
            updateProfile.BindingContext = this.Model;
            await Navigation.PushAsync(updateProfile);
        }

        async void VerifyProfile_Tapped(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(this.Model.EmiratesId)) {
                await DisplayAlert("Profile Verification", "We take the security of our community very seriously. Please upload pictures of your Emirates ID (front & back) in order to start parking. Don't worry these are stored securely on our encrypted servers.", "OK");

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                    PhotoSize = PhotoSize.Small,
                    SaveToAlbum = false
                });

                if (file == null) {
                    return;
                }

                using (MemoryStream stream = new MemoryStream()) {
                    file.GetStream().CopyTo(stream);

                    if (this.Model != null) {
                        this.Model.EmiratesId = Convert.ToBase64String(stream.ToArray());
                        await ServiceUtility.UpdateProfile(this.Model);                        
                        await Navigation.PushAsync(new ProfileVerify());
                    }
                }

            } 
        }


        async void Garage_Tapped(object sender, EventArgs e) {
            var garage = new Garage();
            garage.BindingContext = new GarageModel();
            await Navigation.PushAsync(garage);

        }

        async void Wallet_Tapped(object sender, EventArgs e) {
            var wallet = new Wallet();
            wallet.BindingContext = new WalletModel();
            await Navigation.PushAsync(wallet);

        }

        private void Terms_Tapped(object sender, EventArgs e) {
            Device.OpenUri(new Uri("https://yallaparking.com/home/terms"));
        }

        private void Contact_Tapped(object sender, System.EventArgs e) {
            Device.OpenUri(new Uri("tel:+971566595697"));
        }

		private async void Logout_Tapped(object sender, EventArgs e) {
			PropertyUtility.RemoveKey("token");
			await Navigation.PushAsync(new Login());
		}

	}
}

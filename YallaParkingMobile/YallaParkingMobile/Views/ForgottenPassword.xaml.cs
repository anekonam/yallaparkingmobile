using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class ForgottenPassword : ContentPage {
        public ForgottenPassword() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Forgotten Password Page");
        }
		
		async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, submitting forgotten password request");

            await DisplayAlert ("Connection Error", "Unable to connect to YallaParking, please check your internet connection and try again", "OK");

            Analytics.TrackEvent("Unable to connect to YallaParking services");
        }	

        async void ReturnButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Return button clicked, navigating to Login page");
            await Navigation.PushAsync(new Login());
        }
    }
}

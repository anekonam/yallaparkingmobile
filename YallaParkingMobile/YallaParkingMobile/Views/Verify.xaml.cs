using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class Verify : ContentPage {
        public Verify() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Verify Number Page");
        }    

        async void VerifyButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Verify button clicked, submitting verification code");
                   
            if (string.IsNullOrWhiteSpace(this.VerificationCode.Text)) {
                await DisplayAlert("Verification Code Required", "Please ensure you provide your verification code", "Ok");
                return;
            }

			Activity.IsRunning = true;
			Activity.IsVisible = true;

            var success = await ServiceUtility.Verify(this.VerificationCode.Text);

            Activity.IsRunning = false;
			Activity.IsVisible = false;

            if (success) {
                var profile = await ServiceUtility.Profile();
                var emiratesScan = new EmiratesScan(false);
                emiratesScan.BindingContext = profile;
				await Navigation.PushAsync(emiratesScan);
            } else {
                await DisplayAlert("Verification Failed", "Verification failed, please try again", "Ok");
            }
        }				      
    }
}

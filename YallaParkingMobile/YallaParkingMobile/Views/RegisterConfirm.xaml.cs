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
    public partial class RegisterConfirm : ContentPage {
        public RegisterConfirm() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Verify Number Page");
        }    

        async void VerifyButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Verify button clicked, submitting verification code");
                   
            if (string.IsNullOrWhiteSpace(this.VerificationCode.Text)) {
                await DisplayAlert("Verification Code Required", "Please ensure you provide your verification code", "Ok");
                return;
            } 
            
            var success = await ServiceUtility.RegisterConfirm(this.VerificationCode.Text);

            if (success) {
                await Navigation.PushAsync(new Home());
            } else {
                await DisplayAlert("Verification Failed", "Verification failed, please try again", "Ok");
            }
        }
				
        async void ToolbarItem_Activated(object sender, EventArgs e) {
            await Navigation.PushAsync(new Home());
        }
    }
}

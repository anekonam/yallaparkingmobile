using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class CreateAccount : ContentPage {
        public CreateAccount() {
            InitializeComponent();
            PropertyUtility.SetValue("OnboardingComplete", "true");
            Analytics.TrackEvent("Onboarding Completed");
            Analytics.TrackEvent("Viewing Create Account Page");
        }       

        async void LoginButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Login button clicked, navigating to Login page");
            await Navigation.PushAsync(new Login());
        }

        async void SignUpButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Sign up button clicked, navigating to Registration page");
            await Navigation.PushAsync(new Register());
        }
    }
}

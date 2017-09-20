using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class SignupPage3 : ContentPage {
        public SignupPage3() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing onboarding page 3");
        }        

        async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, navigating to onboarding page 4");
            await Navigation.PushAsync(new SignupPage4());
        }

        async void SkipButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Skip button clicked, navigating to create account page");
            await Navigation.PushAsync(new SignupPage4());
        }
    }
}

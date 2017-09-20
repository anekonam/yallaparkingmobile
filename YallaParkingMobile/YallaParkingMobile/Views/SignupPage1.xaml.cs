using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class SignupPage1 : ContentPage {
        public SignupPage1() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing onboarding page 1");
        }           

        async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked,navigating to onboarding page 2");
            await Navigation.PushAsync(new SignupPage2());
        }

        async void SkipButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Skip button clicked, navigating to create account page");
            await Navigation.PushAsync(new SignupPage4());
        }
    }
}

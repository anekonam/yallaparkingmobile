using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class Login : ContentPage {
        public Login() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Login Page");

            PropertyUtility.SetValue("LoggedIn", "false");
        }

        async void ForgotPasswordButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Forgotten password button clicked, navigating to forgotten password page");

            await Navigation.PushAsync(new ForgottenPassword());
        }
		
		async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, submitting login details");                        

            await Navigation.PushAsync(new Home());
        }
		
		async void FacebookButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Facebook button clicked, submitting Facebook login details");                        

            await Navigation.PushAsync(new Home());
        }

        async void ToolbarItem_Activated(object sender, EventArgs e) {
            await Navigation.PushAsync(new Register());
        }
    }
}

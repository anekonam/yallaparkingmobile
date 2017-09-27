using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
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

            if (string.IsNullOrWhiteSpace(this.EmailAddress.Text)) {
                await DisplayAlert("E-mail Required", "Please ensure you provide your email address", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace(this.Password.Text)) {
                await DisplayAlert("Password Required", "Please ensure you provide your password", "Ok");
                return;
            }

            var model = new LoginModel {                
                EmailAddress = this.EmailAddress.Text,
                Password = this.Password.Text
            };

            Activity.IsRunning = true;
			Activity.IsVisible = true;

            var result = await ServiceUtility.Login(model);

            Activity.IsRunning = false;
            Activity.IsVisible = false;

            if (result.IsSuccessStatusCode) {
                await Navigation.PushAsync(new Home());
            } else if(result.StatusCode == System.Net.HttpStatusCode.Forbidden){
                await DisplayAlert("Verification Required", "Verification required, please enter the code sent to you on the following screen", "Ok");
                await Navigation.PushAsync(new Verify());
            } else {
                await DisplayAlert("Login Failed", "Login failed for the e-mail address and password provided, please try again.", "Ok");                
            }            
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

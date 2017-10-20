using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class ForgottenPassword : ContentPage {
        public ForgottenPassword() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Forgotten Password Page");
        }
		
		async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, submitting forgotten password request");

            if(string.IsNullOrWhiteSpace(EmailAddress.Text)){
                await DisplayAlert("E-mail Address Required", "Please provide your registered e-mail address to reset your password", "Ok");
            } else{
                var response =  await ServiceUtility.ResetPassword(EmailAddress.Text);

                if(response.IsSuccessStatusCode){
                    await DisplayAlert("Password Sent", "A new password has been successfully sent to your registered phone number", "Ok");
                    await Navigation.PopAsync();
                } else{
                    await DisplayAlert("E-mail Address Not Found", "The e-mail address provided is not currently registered, please review and try again", "Ok");
                }
            }
        }	

        async void ReturnButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Return button clicked, navigating to Login page");
            await Navigation.PushAsync(new Login());
        }
    }
}

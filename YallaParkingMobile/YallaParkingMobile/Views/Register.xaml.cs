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
    public partial class Register : ContentPage {
        public Register() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Registration Page");
        }

        private bool IsValidEmail(string email) {            
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, submitting registration details");
                   
            if (string.IsNullOrWhiteSpace(this.Name.Text)) {
                await DisplayAlert("Name Required", "Please ensure you provide your name", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace(this.EmailAddress.Text) || !IsValidEmail(this.EmailAddress.Text)) {
                await DisplayAlert("E-mail Required", "Please ensure you provide a valid e-mail address", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace(this.Password.Text)) {
                await DisplayAlert("Password Required", "Please ensure you provide a password", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace(this.ConfirmPassword.Text) && this.Password.Text == this.ConfirmPassword.Text) {
                await DisplayAlert("Password Required", "Please ensure you confirm your password and that it matches your password", "Ok");
                return;
            } else if (string.IsNullOrWhiteSpace(this.PhoneNumber.Text)) {
                await DisplayAlert("Phone Number Required", "Please ensure you provide your phone number", "Ok");
                return;
            }

            var model = new RegisterModel {
                Name = this.Name.Text,
                EmailAddress = this.EmailAddress.Text,
                Password = this.Password.Text,
                ConfirmPassword = this.ConfirmPassword.Text,
                PhoneNumber = this.PhoneNumberPrefix.Text + this.PhoneNumber.Text
            };

            var success = await ServiceUtility.Register(model);

            if (success) {
                await Navigation.PushAsync(new RegisterConfirm());
            } else {
                await DisplayAlert("Regiser Failed", "Registration failed, please try again", "Ok");
            }
        }
		
		async void FacebookButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Facebook button clicked, submitting Facebook registration details");                        
            //await Navigation.PushAsync(new Home());
        }

        async void ToolbarItem_Activated(object sender, EventArgs e) {
            await Navigation.PushAsync(new Login());
        }
    }
}

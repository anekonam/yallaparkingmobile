﻿using Microsoft.Azure.Mobile.Analytics;
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

            PropertyUtility.SetValue("LoggedIn", "false");
        }

        protected override void OnAppearing() {
            this.Name.Text = null;
            this.EmailAddress.Text = null;
            this.PhoneNumber.Text = null;
            this.Password.Text = null;
            this.ConfirmPassword.Text = null;
        }

        private bool IsValidEmail(string email) {            
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        async void NextButton_Clicked(object sender, EventArgs e) {
            NextButton.IsEnabled = false;

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

			Activity.IsRunning = true;
			Activity.IsVisible = true;

            var result = await ServiceUtility.Register(model);

            NextButton.IsEnabled = true;

            Activity.IsRunning = false;
            Activity.IsVisible = false;

            if (result.IsSuccessStatusCode) {
                await Navigation.PushAsync(new Verify());
            } else if(result.StatusCode == System.Net.HttpStatusCode.Conflict){
                await DisplayAlert("E-mail Address Exists", "The e-mail address provided is already registered, please enter an alternative e-mail address", "Ok");
			} else if (result.StatusCode == System.Net.HttpStatusCode.Forbidden) {
				await DisplayAlert("Phone Number Exists", "The phone number provided is already registered, please enter an alternative phone number", "Ok");
			} else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) {
				await DisplayAlert("Password", "Password must contain atleast 1 upper case letter and 1 number", "Ok");
			} else if (result.StatusCode == System.Net.HttpStatusCode.NotFound) {
				await DisplayAlert("Password", "Your confirmation password must match your password", "Ok");
			} else {
                await DisplayAlert("Registration Failed", "Registration failed, please try again", "Ok");
            }
        }
				
        async void ToolbarItem_Activated(object sender, EventArgs e) {
            await Navigation.PushAsync(new Login());
        }
    }
}

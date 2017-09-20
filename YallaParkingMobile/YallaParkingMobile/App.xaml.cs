﻿using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            var onboarding = PropertyUtility.GetValue("OnboardingComplete") == "true";
            var loggedIn = PropertyUtility.GetValue("LoggedIn") == "true";

            if (loggedIn) {
                Analytics.TrackEvent("Skipping login sequence, navigating to Home");
                MainPage = new NavigationPage(new Home()) {
                    BarTextColor = Color.FromRgb(255, 142, 48)
                };
            } else if (onboarding) {
                Analytics.TrackEvent("Skipping onboarding sequence, navigating to Create Account");
                MainPage = new NavigationPage(new CreateAccount()) {
                    BarTextColor = Color.FromRgb(255,142,48)                    
                };                
            } else {
                Analytics.TrackEvent("Activating onboarding sequence");
                MainPage = new NavigationPage(new SignupPage1()) {                    
                    BarTextColor = Color.FromRgb(255, 142, 48)
                };
            }
        }

        protected override void OnStart() {
            MobileCenter.Start("ios=cb01c0e7-1e13-4db0-a7ab-b6e6bfc6aea3;android=7543c5d7-d484-4a54-a528-26d0bb7744e3", typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("App Started");
        }

        protected override void OnSleep() {
            Analytics.TrackEvent("App Standby");
        }

        protected override void OnResume() {
            Analytics.TrackEvent("App Resumed");
        }
    }
}

using Microsoft.Azure.Mobile.Analytics;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class SignupPage4 : ContentPage {
        public SignupPage4() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing onboarding page 4");
        }        

        async void AcceptButton_Clicked(object sender, EventArgs e) {
            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted) {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location)) {
                        await DisplayAlert("Location Required", "In order to use this application you must be able to share your location.", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted) {
                    await Navigation.PushAsync(new CreateAccount());
                } else if (status != PermissionStatus.Unknown) {
                    await DisplayAlert("Location Denied", "You will not be able to use this application without sharing your location, please change this setting in your phone settings for this application.", "OK");
                }
            } catch (Exception ex) {
                Analytics.TrackEvent(ex.Message);
            }
        }       
    }
}

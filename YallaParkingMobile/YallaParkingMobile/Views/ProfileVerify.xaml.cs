using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class ProfileVerify : ContentPage {
        public ProfileVerify() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Profile Verify Page");
        }        

        async void DoneButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopAsync();
        }
    }
}

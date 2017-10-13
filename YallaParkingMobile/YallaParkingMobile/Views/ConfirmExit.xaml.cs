using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class ConfirmExit : ContentPage {
        public ConfirmExit() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Confirm Exit page");
        }        

        async void YallaButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Yalla button clicked, exiting");
            await Navigation.PopAsync();
        }
    }
}

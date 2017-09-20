using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile {
    public partial class Register : ContentPage {
        public Register() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Registration Page");
        }               
		
		async void NextButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Next button clicked, submitting registration details");                       
            await Navigation.PushAsync(new Home());
        }
		
		async void FacebookButton_Clicked(object sender, EventArgs e) {
            Analytics.TrackEvent("Facebook button clicked, submitting Facebook registration details");                        
            await Navigation.PushAsync(new Home());
        }

        async void ToolbarItem_Activated(object sender, EventArgs e) {
            await Navigation.PushAsync(new Login());
        }
    }
}

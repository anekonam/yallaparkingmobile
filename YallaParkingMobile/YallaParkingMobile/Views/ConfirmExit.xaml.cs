using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;

namespace YallaParkingMobile {
    public partial class ConfirmExit : ContentPage {
        public ConfirmExit() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Confirm Exit page");
        }        

        async void YallaButton_Clicked(object sender, EventArgs e) {
			var model = new BookingsModel();
			var bookings = new Bookings(model);

			await Navigation.PushAsync(bookings);
        }
    }
}

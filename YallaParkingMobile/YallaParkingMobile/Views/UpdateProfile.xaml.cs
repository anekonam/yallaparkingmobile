using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile {
    public partial class UpdateProfile : ContentPage {

        public UpdateProfile() {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Update Profile Page");            
        }

        public ProfileModel ProfileModel {
            get {
                return (ProfileModel)this.BindingContext;
            }                
        }       

    }
}

using Microsoft.Azure.Mobile.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YallaParkingMobile.Model;
using Plugin.Media;
using Plugin.Media.Abstractions;
using YallaParkingMobile.Utility;
using System.IO;

namespace YallaParkingMobile {
    public partial class EmiratesScan : ContentPage {

        private bool back = false;

        public EmiratesScan(){
			InitializeComponent();
        }

        public EmiratesScan(bool back) {
            InitializeComponent();
            Analytics.TrackEvent("Viewing Emirates Scan Page");

            this.back = back;

            if(this.back){
                this.Title = "Scan Back";
                this.Heading.Text = "Scan ID (Back)";
                this.Instruction.Text = "Please take a picture of the back of your ID. If you have used a passport then please take a picture of the front cover";
                this.Instruction1.Text = "";
                this.Instruction2.Text = "";
            } 
        }

		public ProfileModel Model {
			get {
				return (ProfileModel)this.BindingContext;
			}
		}

        async void DoneButton_Clicked(object sender, EventArgs e) {
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
				await DisplayAlert("No Camera", "No camera available.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
				PhotoSize = PhotoSize.Small,
				SaveToAlbum = false
			});

			if (file == null) {
				return;
			}

			using (MemoryStream stream = new MemoryStream()) {
                file.GetStream().CopyTo(stream);

				if (this.Model != null) {
                    if (back) {
                        this.Model.EmiratesIdBack = Convert.ToBase64String(stream.ToArray());
						await Navigation.PushAsync(new ProfileVerify());
                        await ServiceUtility.UpdateProfile(this.Model);
                    } else {
                        this.Model.EmiratesId = Convert.ToBase64String(stream.ToArray());
                        var emiratesScan = new EmiratesScan(true);
                        emiratesScan.BindingContext = this.Model;
                        await Navigation.PushAsync(emiratesScan);
                    }
				}
			}
		}
    }
}

using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using YallaParkingMobile.iOS.Effect;

[assembly: ResolutionGroupName("Effects")]
[assembly: ExportEffect(typeof(SliderEffect), "SliderEffect")]
namespace YallaParkingMobile.iOS.Effect {
    public class SliderEffect : PlatformEffect {
        protected override void OnAttached() {
            var slider = (UISlider)Control;
            slider.SetThumbImage(UIImage.FromFile(("timeSelector.png")), UIControlState.Normal);
            slider.MinimumTrackTintColor = UIColor.FromRGB(255, 142, 48);
        }

        protected override void OnDetached() {
            throw new NotImplementedException();
        }
    }
}

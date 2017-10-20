using System;
using DurianCode.PlacesSearchBar;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YallaParkingMobile.iOS;

[assembly: ExportRenderer(typeof(PlacesBar), typeof(PlacesBarRenderer))]
namespace YallaParkingMobile.iOS {
   public class PlacesBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged( ElementChangedEventArgs<SearchBar> args )
        {
            base.OnElementChanged( args );

            UISearchBar bar = (UISearchBar)this.Control;
            bar.BackgroundColor = UIColor.White;
            bar.BackgroundImage = new UIImage();
            bar.SetSearchFieldBackgroundImage(new UIImage(), UIControlState.Application);
            bar.Translucent = false;
        }
    }
}

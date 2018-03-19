using System;
using DurianCode.PlacesSearchBar;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YallaParkingMobile.Android;
using UIKit;

[assembly: ExportRenderer(typeof(PlacesBar), typeof(PlacesBarRenderer))]
namespace YallaParkingMobile.Android {
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

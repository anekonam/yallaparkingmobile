using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using YallaParkingMobile.Control;
using YallaParkingMobile.Android;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Graphics;

[assembly: Xamarin.Forms.ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace YallaParkingMobile.Android {
    public class BorderlessEntryRenderer : EntryRenderer {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            //Control.SetBackgroundColor(Color.Transparent);
            Control.Background = null;
        }
    }
}
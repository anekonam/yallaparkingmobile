using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using YallaParkingMobile.Control;
using YallaParkingMobile.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Com.Telerik.Android.Common;
using Android.Views;

[assembly: ExportRenderer(typeof(BorderlessPicker), typeof(BorderlessPickerRenderer))]
namespace YallaParkingMobile.Android {
    public class BorderlessPickerRenderer : PickerRenderer {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            Control.Background = null;
            Control.Gravity = GravityFlags.Right;

            var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
            layoutParams.SetMargins(0, 0, 0, 0);
            LayoutParameters = layoutParams;
            Control.LayoutParameters = layoutParams;
            Control.SetPadding(0, 0, 0, 0);
            SetPadding(0, 0, 0, 0);
        }
    }
}
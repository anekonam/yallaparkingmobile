using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using YallaParkingMobile.Control;
using YallaParkingMobile.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BorderlessTimePicker), typeof(BorderlessTimePickerRenderer))]
namespace YallaParkingMobile.Android {
    public class BorderlessTimePickerRenderer : TimePickerRenderer {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            Control.Background = null;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using YallaParkingMobile.Control;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using YallaParkingMobile.Android;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace YallaParkingMobile.Android {
    public class BorderlessDatePickerRenderer : DatePickerRenderer {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            Control.Background = null;
        }
    }
}
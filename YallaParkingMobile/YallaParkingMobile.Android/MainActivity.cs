﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;
using ImageCircle.Forms.Plugin.Droid;
using TK.CustomMap;
using TK.CustomMap.Droid;
using Plugin.Permissions;

namespace YallaParkingMobile.Droid {
    [Activity(Label = "YallaParkingMobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
        protected override void OnCreate(Bundle bundle) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircleRenderer.Init();        
            Xamarin.FormsMaps.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(new App());
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            //TKGoogleMaps.Init(this, bundle);
            MobileCenter.Start("7543c5d7-d484-4a54-a528-26d0bb7744e3", typeof(Distribute));
        }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults) {
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
    }
}


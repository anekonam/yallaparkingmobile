using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;
using ImageCircle.Forms.Plugin.iOS;
using TK.CustomMap.iOSUnified;
using Plugin.Share;

[assembly: ExportRenderer(typeof(Telerik.XamarinForms.Primitives.RadSideDrawer), typeof(Telerik.XamarinForms.PrimitivesRenderer.iOS.SideDrawerRenderer))]
namespace YallaParkingMobile.iOS {
    // The UIApplicationDelegate for the application. C:\Working\YallaParkingMobile\YallaParkingMobile\YallaParkingMobile.iOS\AppDelegate.csThis class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        

        public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
            global::           
            Xamarin.FormsMaps.Init();
            Forms.Init();
            var renderer = new TKCustomMapRenderer();
            ImageCircleRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            Distribute.DontCheckForUpdatesInDebug();

            MobileCenter.Start("cb01c0e7-1e13-4db0-a7ab-b6e6bfc6aea3", typeof(Distribute));

            LoadApplication(new App());

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;

            return base.FinishedLaunching(app, options);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine(e.ExceptionObject.ToString());
        }
    }
}

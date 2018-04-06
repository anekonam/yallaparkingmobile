using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace YallaParkingMobile.Views {
    public partial class MyMapPage : ContentPage {
        public MyMapPage() {
            InitializeComponent();

            var map = new Map(
            MapSpan.FromCenterAndRadius(new Position(25.2048, 55.2708), Distance.FromMiles(0.3))) {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var position = new Position(25.2048, 55.2708); // Latitude, Longitude
            var pin = new Pin {
                Type = PinType.Place,
                Position = position,
                Label = "custom pin",
                Address = "custom detail info",
            };
            map.Pins.Add(pin);

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;
        }
    }
}

using System;
using Android.Content;
using Android.Views;
//using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YallaParkingMobile.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.ViewCell), typeof(StandardViewCellRenderer))]
namespace YallaParkingMobile.Android {
    public class StandardViewCellRenderer : ViewCellRenderer {

        protected override View GetCellCore(Xamarin.Forms.Cell item, View view, ViewGroup viewGroup, Context context) {
            
            var cell = base.GetCellCore(item, view, viewGroup, context);

            cell.Background = null;


            return cell;
        }
    }
}

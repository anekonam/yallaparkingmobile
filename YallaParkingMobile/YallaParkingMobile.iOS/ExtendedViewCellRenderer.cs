using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using YallaParkingMobile.Control;
using YallaParkingMobile.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ViewCell), typeof(ExtendedViewCellRenderer))]
namespace YallaParkingMobile.iOS {
	public class ExtendedViewCellRenderer : ViewCellRenderer {
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv) {
			var cell = base.GetCell(item, reusableCell, tv);
            //var view = item as ExtendedViewCell;
            //cell.SelectedBackgroundView = new UIView {
            //	BackgroundColor = view.SelectedBackgroundColor.ToUIColor(),
            //};
            cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			return cell;
		}

	}
}
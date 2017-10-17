using System;
using Xamarin.Forms;

namespace YallaParkingMobile.Control {
    public class PickerCell : ViewCell {

        private Label _label { get; set; }
        private View _picker { get; set; }
		private Grid _base;

        public string Label {
            get {
                return _label.Text;
            }
            set {
                _label.Text = value;
                _label.HorizontalOptions = LayoutOptions.Start;
                _label.Margin = new Thickness(15, 0, 0, 0);
            }
        }

        public View Picker {
			set {
				//Remove picker if it exists
				if (_picker != null) {
					_base.Children.Remove(_picker);
				}

				//Set its value
				_picker = value;

                //Add to layout
                _base.Children.Add(_picker, 1, 0);
                _picker.HorizontalOptions = LayoutOptions.End;
                _picker.Margin = new Thickness(0, 0, 15, 0);
			}
        }

        public PickerCell():base() {

			_label = new Label() {
				VerticalOptions = LayoutOptions.Center
			};

			_base = new Grid() {
				ColumnDefinitions = new ColumnDefinitionCollection() {
				new ColumnDefinition () { Width = new GridLength (6, GridUnitType.Star) },
				new ColumnDefinition () { Width = new GridLength (6, GridUnitType.Star) }
			}
			};

			_base.Children.Add(_label, 0, 0);

            this.StyleId = "none";
			this.View = _base;

        }

    }
}

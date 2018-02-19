using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {
    public class UserCarModel:INotifyPropertyChanged {
        
        public event PropertyChangedEventHandler PropertyChanged;

		public int? UserCarId { get; set; }

		public ImageSource Image {
			get {
				if (!string.IsNullOrWhiteSpace(this.Make)) {
					return ImageSource.FromFile(string.Format("{0}-logo.png", this.Make.Replace(" ", "-")));
				}

				return null;
			}
		}

		public string Make { get; set; }

		public string ModelNumber { get; set; }

		public string RegistrationNumber { get; set; }

		public string Color { get; set; }

		public string MakeAndModel {
			get {
				return string.Format("{0} {1}", this.Make, this.ModelNumber);
			}
		}

		public string RegAndColor {
			get {
				return string.Format("{0} {1}", this.Color, this.RegistrationNumber);
			}
		}

		private bool isSelected;
		public bool IsSelected {
			get {
				return isSelected;
			}
			set {
                isSelected = value;
                OnPropertyChanged("IsSelected");
			}
		}

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

	}
}

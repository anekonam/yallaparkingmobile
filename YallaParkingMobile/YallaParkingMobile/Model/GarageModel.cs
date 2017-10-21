using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {

	public class GarageModel : INotifyPropertyChanged {

		private ObservableCollection<UserCarModel> userCars = new ObservableCollection<UserCarModel>();
		public ObservableCollection<UserCarModel> UserCars {
			get {
				return userCars;
			}
			set {
				if (userCars != value) {
					userCars = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("UserCars"));
						PropertyChanged(this, new PropertyChangedEventArgs("HasCars"));
						PropertyChanged(this, new PropertyChangedEventArgs("HasNoCars"));
					}
				}
			}
		}

		public bool HasCars {
			get {
				return this.UserCars != null && this.UserCars.Any();
			}
		}

		public bool HasNoCars {
			get {
				return !this.HasCars;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

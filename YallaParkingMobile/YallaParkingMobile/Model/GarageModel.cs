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
					}
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

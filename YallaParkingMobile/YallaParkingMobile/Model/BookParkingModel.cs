using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {

    public class BookParkingModel : INotifyPropertyChanged {

        public BookParkingModel(PropertyModel property) {
            this.Property = property;
        }

        public PropertyModel Property { get; set; }

        private UserCarModel selectedUserCar;
		public UserCarModel SelectedUserCar {
			get {
				return selectedUserCar;
			}
			set {
				if (selectedUserCar != value) {
					selectedUserCar = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedUserCar"));
                        PropertyChanged(this, new PropertyChangedEventArgs("CanBook"));
					}
				}
			}
		}

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

        public bool CanBook{
            get{
                return this.Property != null && this.SelectedUserCar != null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

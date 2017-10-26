using System;
using System.ComponentModel;
namespace YallaParkingMobile.Model {
    public class HomeModel:INotifyPropertyChanged {

        public HomeModel(bool parkNow = true){
            this.ParkNow = parkNow;
        }

        private bool parkNow;
        public bool ParkNow{
            get{
                return parkNow;
            } set{
                if(parkNow!=value){
                    parkNow = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkNow"));
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkLater"));
                    }
                }
            }
        }

        public bool ParkLater{
            get{
                return !this.ParkNow;
            }
        }

        private PropertyModel selectedProperty;
        public PropertyModel SelectedProperty{
            get{
                return selectedProperty;
            } set{
                if(selectedProperty!=value){
                    selectedProperty = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectedProperty"));
                        PropertyChanged(this, new PropertyChangedEventArgs("HasSelectedProperty"));
                        PropertyChanged(this, new PropertyChangedEventArgs("NotSelectedProperty"));
                    }
                }
            }
        }

		private bool allDay;
		public bool AllDay {
			get {
				return allDay;
			}
			set {
				if (allDay != value) {
					allDay = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("AllDay"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Hourly"));
					}
				}
			}
		}

        public bool Hourly{
            get{
                return !this.AllDay;
            }
        }

        public bool HasSelectedProperty{
            get{
                return this.SelectedProperty != null;
            }
        }

        public bool NotSelectedProperty{
            get{
                return !this.HasSelectedProperty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

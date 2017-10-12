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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

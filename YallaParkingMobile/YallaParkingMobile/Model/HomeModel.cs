using System;
using System.ComponentModel;
namespace YallaParkingMobile.Model {
    public class HomeModel:INotifyPropertyChanged {

        public HomeModel(){
            
        }

        private bool parkNow;
        public bool ParkNow{
            get{
                return parkNow;
            } set{
                if(parkNow!=value){
                    parkNow = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ParkNow"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

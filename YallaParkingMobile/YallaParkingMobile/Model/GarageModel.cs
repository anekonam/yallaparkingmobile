using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {
    
    public class GarageModel:INotifyPropertyChanged {

        private ObservableCollection<UserCarModel> userCars = new ObservableCollection<UserCarModel>();
        public ObservableCollection<UserCarModel> UserCars{
            get{
                return userCars;
            }
            set{
                if(userCars!=value){
                    userCars = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("UserCars"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class UserCarModel {

        public int? UserCarId { get; set; }

        public ImageSource Image{
            get{
                if(!string.IsNullOrWhiteSpace(this.Make)){
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
            get{
                return string.Format("{0} {1}", this.Make, this.ModelNumber);
            }
        }

        public string RegAndColor{
            get{
                return string.Format("{0} {1}", this.Color, this.RegistrationNumber);
            }
        }

    }
}

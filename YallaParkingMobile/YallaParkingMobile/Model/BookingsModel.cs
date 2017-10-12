using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using YallaParkingMobile.Model;
using YallaParkingMobile.Utility;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;

namespace YallaParkingMobile.Model {
    public class BookingsModel:INotifyPropertyChanged {

        public async Task GetBookings(){
            this.IsBusy = true;

            var bookingResult = await ServiceUtility.GetBookings();
            this.Bookings = new ObservableCollection<BookingModel>(bookingResult);

            this.IsBusy = false;
        }

        private bool isBusy;
        public bool IsBusy{
            get{
                return isBusy;
            } set{
                if(isBusy!=value){
                    isBusy = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Bookings"));
					}
                }
            }
        }

		private ObservableCollection<BookingModel> bookings = new ObservableCollection<BookingModel>();
		public ObservableCollection<BookingModel> Bookings {
			get {
				return bookings;
			}
			set {
				if (bookings != value) {
					bookings = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Bookings"));
                        PropertyChanged(this, new PropertyChangedEventArgs("HasBookings"));
                        PropertyChanged(this, new PropertyChangedEventArgs("HasNoBookings"));
					}
				}
			}
		}

        public bool HasBookings{
            get{
                return this.Bookings != null && this.Bookings.Any();
            }
        }

		public bool HasNoBookings {
			get {
                return !this.HasBookings;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
    }
}

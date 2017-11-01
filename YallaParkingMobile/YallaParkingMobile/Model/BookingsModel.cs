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

        private bool isBusy = true;
        public bool IsBusy{
            get{
                return isBusy;
            } set{
                if(isBusy!=value){
                    isBusy = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
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
                        PropertyChanged(this, new PropertyChangedEventArgs("BookingsGrouped"));
					}
				}
			}
		}

        public ObservableCollection<Grouping<string, BookingModel>> BookingsGrouped{
            get{
                if(this.Bookings!=null){
                    var bookingsGrouped = this.Bookings
                                              .OrderByDescending(b => b.Start)
                                              .GroupBy(b => b.Status)
                                              .OrderBy(b => b.Key)
                                              .Select(b => new Grouping<string, BookingModel>(b.Key.Split(',')[1], b));
                    
                    return new ObservableCollection<Grouping<string, BookingModel>>(bookingsGrouped);
                }

                return null;
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

    public class Grouping<K, T> : ObservableCollection<T> {

        public K Key { get; set; }

        public Grouping(K key, IEnumerable<T> items){
            Key = key;

            foreach(var item in items){
                this.Items.Add(item);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using YallaParkingMobile.Utility;

namespace YallaParkingMobile.Model {

    public class BookParkingModel : INotifyPropertyChanged {

        public BookParkingModel(PropertyModel property, bool parkingNow = true) {
            this.Property = property;
            this.ParkingNow = parkingNow;
        }

        private bool parkingNow = true;
        public bool ParkingNow{
            get{
                return parkingNow;
            } set{
                if(parkingNow!=value){
                    parkingNow = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
                        PropertyChanged(this, new PropertyChangedEventArgs("BookText"));
                    }
                }
            }
        }

        public bool Booking{
            get{
                return !this.ParkingNow;
            }
        }

        public decimal Discount { get; set; }

        private PropertyModel property = new PropertyModel();
		public PropertyModel Property {
			get {
				return property;
			}
			set {
				if (property != value) {
					property = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Property"));
					}
				}
			}
		}

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
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
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

		private UserCardModel selectedUserCard;
		public UserCardModel SelectedUserCard {
			get {
				return selectedUserCard;
			}
			set {
				if (selectedUserCard != value) {
					selectedUserCard = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedUserCard"));
						PropertyChanged(this, new PropertyChangedEventArgs("CanBook"));
						PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
						PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
					}
				}
			}
		}

		private ObservableCollection<UserCardModel> userCards = new ObservableCollection<UserCardModel>();
		public ObservableCollection<UserCardModel> UserCards {
			get {
				return userCards;
			}
			set {
				if (userCards != value) {
					userCards = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("UserCards"));
					}
				}
			}
		}

        public string BookText{
            get{
                return this.ParkingNow ? "Park Now" : "Book Now";
            }
        }

        public bool CanBook{
            get{
                return this.Property != null && this.SelectedUserCar != null && this.SelectedUserCard!=null;
            }
        }

        public string BookingNumber { get; set; }

        public async Task<bool> BookParking(){
            var model = new BookingModel {
                UserCarId = this.SelectedUserCar.UserCarId.Value,
                UserCardId = this.SelectedUserCard.UserCardId.Value,
                PropertyId = this.Property.PropertyId,
                Start = this.Property.StartDate,
                End = this.Property.EndDate,
                Price = this.Property.ShortTermParkingPrice,
                Discount = this.Discount,
                Hours = this.Property.Hours
            };

            this.BookingNumber = await ServiceUtility.Book(model);
            this.BookingNumber = string.Format("#{0}", this.BookingNumber).Replace("\"", "");

            return !string.IsNullOrWhiteSpace((this.BookingNumber));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

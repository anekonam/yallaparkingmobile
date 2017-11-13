using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using YallaParkingMobile.Utility;
using Plugin.Geolocator.Abstractions;
using System.Windows.Input;

namespace YallaParkingMobile.Model {

    public class BookParkingModel : INotifyPropertyChanged {

        public BookParkingModel(PropertyModel property, bool parkNow, bool parkingNow = true) {
            this.Property = property;
            this.ParkingNow = parkingNow;
            this.ParkNow = parkNow;
        }

        public bool ParkNow { get; set; }

        private bool parkingNow = true;
        public bool ParkingNow {
            get {
                return parkingNow;
            }
            set {
                if (parkingNow != value) {
                    parkingNow = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
                        PropertyChanged(this, new PropertyChangedEventArgs("BookText"));
                    }
                }
            }
        }

        public bool Booking {
            get {
                return !this.ParkingNow;
            }
        }

        public string Description {
            get {
                var description = "* This is an approximate order summary. The actual cost will";
                return description;
            }
        }

        public string Description1 {
            get {
                var description1 = "be calculated based on your scan in & out time *";
                return description1;
            }
        }

        public bool HasDiscount {
            get {
                return this.Property.Discount > 0;
            }
        }

        private decimal discount;
		public decimal Discount {
            get {
                return discount;
            }
            set {
                if (discount != value) {
                    discount = value;

                    if (PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("Discount"));
                    }
                }
            }
        }

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

		public string EntranceMethod {
			get {
                if (!string.IsNullOrWhiteSpace(this.Property.ShortTermParkingEntranceMethod)){
					return this.Property.ShortTermParkingEntranceMethod;
				} else {
					return "N/A";
				}
			}
		}

		public string AccessInfo {
			get {
                if (!string.IsNullOrWhiteSpace(this.Property.ShortTermParkingAccessInfo)){
					return this.Property.ShortTermParkingAccessInfo;
				} else {
					return "N/A";

				}
			}
		}

		public string Details {
			get {
                if (!string.IsNullOrWhiteSpace(this.Property.ShortTermParkingDetails)){
					return this.Property.ShortTermParkingDetails;
				} else {
					return "N/A";
				}
			}
		}
      public string TotalHours {
			get {
				if (this.Property.Hours < 8) {
					return this.Property.Hours.ToString();
				} else if (this.Property.Hours >= 8) {
					return "All Day";
				}

				return this.Property.Hours.ToString();
			}
		}

        public int BufferMinutes{
            get{
                return this.Property.ShortTermParkingEntryTimeBufferMinutes.HasValue ? this.Property.ShortTermParkingEntryTimeBufferMinutes.Value : 0;
            }
        }

        public bool AllDay{
            get{
                return this.ParkNow == true ? this.Property.Hours >= 1 : this.Property.Hours >= 8;
            }
        }

        public bool Hourly{
            get{
                return !AllDay;
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

        public int TotalCar { get; set; }

        public int TotalCard { get; set; }

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
                Start = this.Property.StartDate.ToUniversalTime(),
                End = this.Property.EndDate.ToUniversalTime(),
                Price = this.ParkNow && this.AllDay? this.Property.Hours >= 1 ? this.Property.ShortTermParkingFullDayPrice : this.Property.ShortTermParkingPrice : this.Property.Hours >= 8 ? this.Property.ShortTermParkingFullDayPrice : this.Property.ShortTermParkingPrice,
                Discount = this.Discount,
                Hours = this.Property.Hours,
                ParkNow = this.ParkNow,
                AllDay = this.AllDay
            };

            this.BookingNumber = await ServiceUtility.Book(model);
            this.BookingNumber = string.Format("#{0}", this.BookingNumber).Replace("\"", "");

            return !string.IsNullOrWhiteSpace((this.BookingNumber));
        }

        public event PropertyChangedEventHandler PropertyChanged;


		public double? Distance {
			get {
                return this.CurrentLocation != null && this.Property.Latitude.HasValue && this.Property.Longitude.HasValue ? 
                           this.CurrentLocation.CalculateDistance(new Position(this.Property.Latitude.Value, this.Property.Longitude.Value)) : (double?)null;
			}
		}

		public bool HasDistance {
			get {
				return this.Distance.HasValue;
			}
		}       

        public bool IsParkNow {
            get {
                if (!this.ParkNow) {
                    return true;
                } else {
                    return false;
                }
            }
        }

		private Position currentLocation;
		public Position CurrentLocation {
			get {
				return currentLocation;
			}
			set {
				if (currentLocation != value) {
					currentLocation = value;

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("CurrentLocation"));
						PropertyChanged(this, new PropertyChangedEventArgs("Distance"));
                        PropertyChanged(this, new PropertyChangedEventArgs("HasDistance"));
					}
				}
			}
		}
    }

   
}

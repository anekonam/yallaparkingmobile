﻿using System;
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
using System.Net.Http;

namespace YallaParkingMobile.Model {

    public class BookParkingModel : INotifyPropertyChanged {

        public BookParkingModel(PropertyModel property, bool parkNow, bool allDay, bool parkingNow = true) {
            this.Property = property;
            this.ParkingNow = parkingNow;
            this.ParkNow = parkNow;
            this.AllDay = allDay;
        }

        public async Task SetBookingDefaults(){
            var bookings = await ServiceUtility.GetBookings();

            if(bookings!=null && bookings.Any()){
                var lastBooking = bookings.OrderByDescending(b => b.Created).FirstOrDefault();

                if (this.SelectedUserCar == null) {
                    foreach (var userCar in this.UserCars) {
                        if (lastBooking.UserCarId == userCar.UserCarId) {
                            this.SelectedUserCar = userCar;
                        }
                    }
                } else{
                    UpdateSelectedCar();
                }

                if (this.SelectedUserCard == null) {
                    foreach (var userCard in this.UserCards) {
                        if (lastBooking.UserCardId == userCard.UserCardId) {
                            this.SelectedUserCard = userCard;
                        }
                    }
                } else{
                    UpdateSelectedCard();
                }
            }
        }

        public bool ParkNow { get; set; }

        public bool ParkNowAllDay { 
            get{
                return this.ParkNow && !this.AllDay;

            }
        }

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

		public string VatMessage {
			get {
				return "* 5% VAT will be added at checkout ";
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

        public string FreeMinutes{
            get{
                return property.ShortTermParkingFreeMinutes.HasValue ? string.Format("* {0} mins free parking",property.ShortTermParkingFreeMinutes.Value) : string.Empty;
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
				if (!this.AllDay) {
					return this.Property.Hours.ToString();
				} else {
                    return "All Day (Until Midnight)";
				}
			}
		}

        public double TotalParkingHours { get; set; }

        public int BufferMinutes{
            get{
                return this.Property.ShortTermParkingEntryTimeBufferMinutes.HasValue ? this.Property.ShortTermParkingEntryTimeBufferMinutes.Value : 0;
            }
        }

        public bool AllDay {get; set;}

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

                    UpdateSelectedCar();

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedUserCar"));
                        PropertyChanged(this, new PropertyChangedEventArgs("CanBook"));
                        PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
					}
				}
			}
		}
        private void UpdateSelectedCar(){
            foreach(var car in UserCars){
                car.IsSelected = car.UserCarId == selectedUserCar.UserCarId;
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

                    UpdateSelectedCard();

					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("SelectedUserCard"));
						PropertyChanged(this, new PropertyChangedEventArgs("CanBook"));
						PropertyChanged(this, new PropertyChangedEventArgs("ParkingNow"));
						PropertyChanged(this, new PropertyChangedEventArgs("Booking"));
					}
				}
			}
		}

		private void UpdateSelectedCard() {
			foreach (var card in UserCards) {
                card.IsSelected = card.UserCardId == selectedUserCard.UserCardId;
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

		public string ButtonText {
			get {
				return this.ParkNow ? "SCAN IN - Yalla I'm Here" : "Confirm Booking";
			}
		}

        public bool CanBook{
            get{
                return this.Property != null && this.SelectedUserCar !=null && this.SelectedUserCard != null;
            }
        }

        public decimal ParkingPrice{
            get{
                if(this.ParkNow && this.AllDay){
                    return this.Property.ShortTermParkingFullDayPrice;
                } else if (this.ParkNow && !this.AllDay){
                    return this.Property.ShortTermParkingPrice;
                } else if(!this.ParkNow && this.AllDay){
                    return this.Property.ShortTermParkingFullDayPrice;
				} else if (!this.ParkNow && !this.AllDay) {
					return this.Property.ShortTermParkingPrice;
				}
                return 0;
			}
        }

        public double ParkingHours { get; set; }

		public decimal TotalPrice {
			get {
				if (this.AllDay) {
					return this.Property.ShortTermParkingFullDayPrice - this.Property.Discount;
				}

				return ((decimal)this.ParkingHours * this.Property.ShortTermParkingPrice) - this.Property.Discount;
			}
		}

        public string BookingNumber { get; set; }

        public string DiscountCode { get; set; }

        public async Task<HttpResponseMessage> BookParking(){
            var model = new BookingModel {
                UserCarId = this.SelectedUserCar != null ? this.SelectedUserCar.UserCarId : null,
                UserCardId = this.SelectedUserCard != null ? this.SelectedUserCard.UserCardId : null,
                PropertyId = this.Property.PropertyId,
                Start = this.Property.StartDate.ToUniversalTime(),
                End = this.Property.EndDate.ToUniversalTime(),
                Price = this.ParkingPrice,
                Discount = this.Discount,
                Hours = this.Property.Hours,
                ParkNow = this.ParkNow,
                AllDay = this.AllDay,
                DiscountCode = this.DiscountCode
            };

            return await ServiceUtility.Book(model);
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

        public bool IsParkLater {
            get {
                if (!this.ParkNow && !this.AllDay) {
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

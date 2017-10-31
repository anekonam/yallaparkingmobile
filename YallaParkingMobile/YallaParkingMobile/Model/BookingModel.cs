using System;
using Humanizer;
using Xamarin.Forms;
using System.ComponentModel;
using Plugin.Geolocator.Abstractions;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace YallaParkingMobile.Model {
    public class BookingModel:INotifyPropertyChanged {

        public int PropertyParkingId { get; set; }

        public int? ValidatorUserId { get; set; }

        public int[] ValidatorUserIds { get; set; }

        public int UserCarId { get; set; }

        public int UserCardId { get; set; }

        public string UserCardBrand { get; set; }

		public ImageSource UserCardImage {
			get {
				if (!string.IsNullOrWhiteSpace(this.UserCardBrand)) {
					return ImageSource.FromFile(string.Format("{0}-logo.png", this.UserCardBrand.Replace(" ", "-")));
				}

				return null;
			}
		}

		public string UserCardName { get; set; }

		public int UserCardExpireMonth { get; set; }

		public int UserCardExpireYear { get; set; }

		public string UserCardExpireMonthYear {
			get {
				return string.Format("{0}/{1}", this.UserCardExpireMonth, this.UserCardExpireYear);
			}
		}
        		

		public string UserCardLastFourDigits { get; set; }

		public string UserCardEncodedCardNumber {
			get {
				return string.Format("**** **** **** {0}", this.UserCardLastFourDigits).Replace('*', '\u2022');
			}
		}

        public ImageSource UserCarImage {
            get {
                if (!string.IsNullOrWhiteSpace(this.UserCarMake)) {
                    return ImageSource.FromFile(string.Format("{0}-logo.png", this.UserCarMake.Replace(" ", "-")));
                }

                return null;
            }
        }

        public string UserCarMake { get; set; }

        public string UserCarModelNumber { get; set; }

        public string UserCarColor { get; set; }

        public string UserCarRegistrationNumber { get; set; }

        public string UserCarMakeAndModel {
            get {
                return string.Format("{0} {1}", this.UserCarMake, this.UserCarModelNumber);
            }
        }

        public string UserCarRegAndColor {
            get {
                return string.Format("{0} {1}", this.UserCarColor, this.UserCarRegistrationNumber);
            }
        }

        public int PropertyId { get; set; }

        public ImageSource PropertyImage {
            get {
                return ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/image/" + this.PropertyId.ToString()));
            }
        }

        public string PropertyName { get; set; }

        public string PropertyPropertyArea { get; set; }

        public string PropertyCity { get; set; }

        public double PropertyLatitude { get; set; }

        public double PropertyLongitude { get; set; }

        public int? PropertyShortTermParkingEntryBufferMinutes { get; set; }

        public string PropertyShortTermParkingEntranceMethod { get; set; }

        public string PropertyShortTermParkingAccessInfo { get; set; }

        public string PropertyShortTermParkingDetails { get; set; }

        public int? PropertyShortTermParkingCancelMinutes { get; set; }

        public string EntranceMethod{
            get{
                if(!string.IsNullOrWhiteSpace(this.PropertyShortTermParkingEntranceMethod)){
                    return this.PropertyShortTermParkingEntranceMethod;
                } else{
                    return "N/A";
                }
            }
        }

        public string AccessInfo {
			get {
                if (!string.IsNullOrWhiteSpace(this.PropertyShortTermParkingAccessInfo)){
					return this.PropertyShortTermParkingAccessInfo;
				} else {
                    return "N/A";

				}
			}
		}

		public string Details {
			get {
                if (!string.IsNullOrWhiteSpace(this.PropertyShortTermParkingDetails)){
					return this.PropertyShortTermParkingDetails;
				} else {
                    return "N/A";
				}
			}
		}


        private string number;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Number {
            get {
                return string.Format("#{0}", number);
            }
            set {
                if (number != value) {
                    number = value;
                }
            }
        }

        public DateTime Start { get; set; }

        public DateTime StartLocal{
            get{
                return this.Start.ToLocalTime();
            }
        }

        public DateTime End { get; set; }

        public DateTime EndLocal{
            get{
                return this.End.ToLocalTime();
            }
        }

        public string RemainingTime {
            get {
                if (this.End > DateTime.UtcNow) {
                    var timeSpan = this.End - DateTime.UtcNow;
                    var totalHours = timeSpan.TotalHours >= 1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("HH").Replace("0", "")) : 0;
                    var minutes = timeSpan.TotalMinutes >= 1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("mm")) : 0;
                    return string.Format("{0} {1} {2} {3} left", totalHours, totalHours == 1 ? "hr" : "hrs", minutes, minutes == 1 ? "min" : "mins");
                }

                return "Ended";
            }
        }


		public string BookingTime {
			get {
				if (this.Start.Date == DateTime.UtcNow.Date) {
					return String.Join(" to ", String.Format("{0} {1: HH:mm}", "Today", this.StartLocal), String.Format("{0: HH:mm}", this.EndLocal));
				} else {
					return String.Join(" to ", String.Format("{0} {1} {2:MMM HH:mm}", this.StartLocal.ToString("ddd"), this.StartLocal.Day.Ordinalize(), this.StartLocal),
									   String.Format("{0} {1:MMM HH:mm}", this.EndLocal.Day.Ordinalize(), this.EndLocal));
				}
			}
		}

        public DateTime? EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        public string Status {
            get {
                if (this.EntryTime.HasValue && !this.ExitTime.HasValue) {
                    return "1,Active";
                } else if (this.EntryTime.HasValue && this.ExitTime.HasValue) {
                    return "3,Completed";
                } else if(this.Cancelled.HasValue){
                    return "4, Cancelled";
                }

                return "2,Upcoming";
            }
        }

        public string ParkingTime{
            get{
                if(this.EntryTime.HasValue && this.ExitTime.HasValue){
                    TimeSpan difference = this.ExitTime.Value - this.EntryTime.Value;
                    return string.Format("You parked for {0} hr {1} mins", difference.Hours, difference.Minutes);
                }

                return string.Empty;
            }
        }

        public string ParkingCharge{
            get{
                if (this.TotalPrice.HasValue) {
                    var price = this.TotalPrice.Value.ToString("n2");
                    return string.Format("You will be charged AED {0}", price);
                }

                return null;
            }
        }

        public string ParkingCompleteDetails{
            get{
                if(this.Completed && this.TotalPrice.HasValue && this.EntryTime.HasValue && this.ExitTime.HasValue){
                    var price = this.TotalPrice.Value.ToString("n2");
                    TimeSpan difference = this.ExitTime.Value - this.EntryTime.Value;

                    return string.Format("You parked for {0} hr {1} mins and you paid AED {2}", difference.Hours, difference.Minutes, price);
				}
				return null;
            }
        }

        public bool Validated {
            get {
                return !this.Cancelled.HasValue && this.ValidatorUserId.HasValue;
            }
        }

        public bool Discounted {
            get {
                return this.Discount.HasValue && this.Discount.Value > 0 && !this.ValidatorUserId.HasValue;
            }
        }

        public bool CanValidate {
            get {
                return !this.Cancelled.HasValue && !this.ValidatorUserId.HasValue && (!this.Discount.HasValue || this.Discount.Value == 0);
            }
        }

        public bool CanEnter {
            get {
                return !this.Cancelled.HasValue && !this.Cancelled.HasValue && !this.EntryTime.HasValue;
            }
        }

        public bool CanExit {
            get {
                return !this.Cancelled.HasValue && this.EntryTime.HasValue && !this.ExitTime.HasValue;
            }
        }

        public bool CanCancel {
            get {
                return !this.Cancelled.HasValue && !this.EntryTime.HasValue && !this.ExitTime.HasValue;
            }
        }

		public bool IsCancelled {
			get {
				return this.Status.Contains("Cancelled");
			}
		}

        public bool Pending {
            get {
                return this.Status.Contains("Upcoming") || this.Status.Contains("Active");
            }
        }

        public bool CancellationMessage{
            get{
                return this.Status.Contains("Upcoming");
            }
        }

        public bool Active {
            get {
                return this.Status.Contains("Active");
            }
        }

        public bool Completed {
            get {
                return this.Status.Contains("Completed");
            }
        }

        public bool canEdit {
            get {
                return this.Status.Contains("Upcoming");
            }
        }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal DiscountValue {
            get {
                return this.Discount.HasValue && this.Hours.HasValue ? (decimal)this.Hours.Value * this.Price * (this.Discount.Value / 100) : 0;
            }
        }

        public double? OriginalHours { get; set; }

        private double? hours;
        public double? Hours {
            get{
                return hours;
            } set{
                if(hours!=value){
                    hours = value;
					if (PropertyChanged != null) {
						PropertyChanged(this, new PropertyChangedEventArgs("Hours"));
					}
				
                }
            }
        }

		public string TotalHours { 
            get{
                if(this.Hours.HasValue && this.Hours < 8){
                    return this.Hours.ToString();
                } else if (this.Hours.HasValue && this.Hours >= 8){
                    return "All Day";
                }

                return this.Hours.ToString();
            }
        }

        public decimal? EstimatedTotalPrice {
            get {
                if(this.Hours.HasValue){
                    if(this.Hours.Value >= 8){
                        return this.Price;
                    } else{
                        return this.Price * (decimal)this.Hours.Value;
                    }
                }

                return (decimal?)null;
            }
        }

        public decimal? TotalPrice { get; set; }

        public decimal? Total{
            get{
                if (this.Cancelled.HasValue) {
                    return this.CancellationCharge.HasValue ? this.CancellationCharge.Value : 0.0M;
                } 

                return this.Completed ? this.TotalPrice : this.EstimatedTotalPrice;
            }
        }

		public string CompletedDetail {
			get {
                return this.ExitTime.HasValue ? string.Format("Completed {0} {1} {2:MMM HH:mm}", this.ExitTime.Value.ToLocalTime().ToString("ddd"), this.ExitTime.Value.ToLocalTime().Day.Ordinalize(), this.ExitTime.Value.ToLocalTime()) : null;
			}
		}

        public DateTime? Cancelled { get; set; }

        public string CancelledDetail{
            get{
                return this.Cancelled.HasValue ? string.Format("Cancelled {0} {1} {2:MMM HH:mm}", this.Cancelled.Value.ToLocalTime().ToString("ddd"), this.Cancelled.Value.ToLocalTime().Day.Ordinalize(), this.Cancelled.Value.ToLocalTime()) : null;
            }
        }

		public string CancellationPolicy {
			get {
                return string.Format("At least {0} mins prior to your booking",this.PropertyShortTermParkingCancelMinutes);
			}
		}

        public decimal? CancellationCharge { get; set; }

        public double? Distance{
            get{
                return this.CurrentLocation != null ? this.CurrentLocation.CalculateDistance(new Position(this.PropertyLatitude, this.PropertyLongitude)) : (double?)null;
            }
        }

        public bool HasDistance{
            get{
                return this.Distance.HasValue;
            }
        }

        private Position currentLocation;
        public Position CurrentLocation {
            get {
                return currentLocation;
            } set{
                if(currentLocation!=value){
                    currentLocation = value;

                    if(PropertyChanged!=null){
						PropertyChanged(this, new PropertyChangedEventArgs("CurrentLocation"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Distance"));
						PropertyChanged(this, new PropertyChangedEventArgs("HasDistance"));
                    }
                }
            }
        }

        public DateTime Created { get; set; }
    }
}

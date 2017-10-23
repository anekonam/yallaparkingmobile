using System;
using Humanizer;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {
    public class BookingModel {

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

        public int PropertyEntryBufferMinutes { get; set; }

        private string number;
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
                    var hours = timeSpan.TotalHours >= 1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("HH").Replace("0", "")) : 0;
                    var minutes = timeSpan.TotalMinutes >= 1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("mm")) : 0;
                    return string.Format("{0} {1} {2} {3} left", hours, hours == 1 ? "hr" : "hrs", minutes, minutes == 1 ? "min" : "mins");
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

                return "2,Pending";
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
                return this.Status.Contains("Pending");
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



        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal DiscountValue {
            get {
                return this.Discount.HasValue && this.Hours.HasValue ? (decimal)this.Hours.Value * this.Price * (this.Discount.Value / 100) : 0;
            }
        }

        public double? Hours { get; set; }

        public decimal? EstimatedTotalPrice {
            get {
                return this.Hours.HasValue ? this.Price * (decimal)this.Hours.Value : 0.0M;
            }
        }

        public decimal? TotalPrice { get; set; }

        public decimal? Total{
            get{
                return this.Completed ? this.TotalPrice : this.EstimatedTotalPrice;
            }
        }

        public DateTime? Cancelled { get; set; }

        public string CancelledDetail{
            get{
                return this.Cancelled.HasValue ? string.Format("Cancelled {0} {1} {2:MMM HH:mm}", this.Cancelled.Value.ToLocalTime().ToString("ddd"), this.Cancelled.Value.ToLocalTime().Day.Ordinalize(), this.Cancelled.Value.ToLocalTime()) : null;
            }
        }

        public decimal? CancellationCharge { get; set; }
    }
}

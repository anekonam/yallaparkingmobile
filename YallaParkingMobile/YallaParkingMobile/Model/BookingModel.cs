using System;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {
    public class BookingModel {

        public int PropertyParkingId { get; set; }

        public int? ValidatorUserId { get; set; }

        public int[] ValidatorUserIds { get; set; }

        public int UserCarId { get; set; }

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
            get{
                return ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/image/" + this.PropertyId.ToString()));
            }
        }

        public string PropertyName { get; set; }

        public string PropertyPropertyArea { get; set; }

        public string PropertyCity { get; set; }

        private string number;
        public string Number {
            get{
                return string.Format("#{0}", number);
            } set{
                if(number!=value){
                    number = value;
                }
            }
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string RemainingTime{
            get{
                if(this.End > DateTime.Now){
                    var timeSpan = this.End - DateTime.Now;
                    var hours = timeSpan.TotalHours >=1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("HH").Replace("0", "")) : 0;
                    var minutes = timeSpan.TotalMinutes >= 1 ? int.Parse(new DateTime(timeSpan.Ticks).ToString("mm")) : 0;
                    return string.Format("{0} {1} {2} {3} left", hours, hours == 1 ? "hr" : "hrs", minutes, minutes == 1 ? "min" : "mins");
                }

                return "Ended";
            }
        }

		public string BookingTime {
			get {
				return string.Format("{0} to {1}", this.Start.ToString("dd-MMM-yy HH:mm"), this.End.ToString("dd-MMM-yy HH:mm"));
			}
		}

        public DateTime? EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        public string Status{
            get{
                if(this.EntryTime.HasValue && !this.ExitTime.HasValue){
                    return "1,Active";
                } else if(this.EntryTime.HasValue && this.ExitTime.HasValue){
                    return "3,Completed";
                }

                return "2,Pending";
            }
        }

        public bool Validated{
            get{
                return this.ValidatorUserId.HasValue;
            }
        }

        public bool Discounted{
            get{
                return this.Discount.HasValue && this.Discount.Value > 0 && !this.ValidatorUserId.HasValue;
            }
        }

        public bool CanValidate{
            get{
                return !this.ValidatorUserId.HasValue && (!this.Discount.HasValue || this.Discount.Value == 0);
            }
        }

        public bool CanEnter{
            get{
                return !this.EntryTime.HasValue;
            }
        }

        public bool CanExit{
            get{
                return this.EntryTime.HasValue && !this.ExitTime.HasValue;
            }
        }

        public bool CanCancel{
            get{
                return !this.EntryTime.HasValue && !this.ExitTime.HasValue;
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

        public bool Completed{
            get{
                return this.Status.Contains("Completed");
            }
        }

		

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public decimal DiscountValue{
            get{
                return this.Discount.HasValue && this.Hours.HasValue ? (decimal)this.Hours.Value * this.Price * (this.Discount.Value / 100) : 0;
            }
        }

        public double? Hours { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}

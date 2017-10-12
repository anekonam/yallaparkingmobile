using System;
using Xamarin.Forms;

namespace YallaParkingMobile.Model {
    public class BookingModel {

        public int PropertyParkingId { get; set; }

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

        public string Number { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

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
                    return "Active";
                } else if(this.EntryTime.HasValue && this.ExitTime.HasValue){
                    return "Completed";
                }

                return "Pending";
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

        public bool Completed{
            get{
                return this.EntryTime.HasValue && this.ExitTime.HasValue;
            }
        }

		public bool NotCompleted {
			get {
                return !this.Completed;
			}
		}

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public double? Hours { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}

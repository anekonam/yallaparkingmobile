using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Humanizer;

namespace YallaParkingMobile.Model {

    public class PropertyModel : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public int PropertyId { get; set; }

        public string AreaName { get; set; }

        public string Name { get; set; }

        public string PropertyArea { get; set; }

        public string City { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public decimal ShortTermParkingPrice { get; set; }

        public decimal ShortTermParkingFullDayPrice { get; set; }

        public string ShortTermParkingEntranceMethod { get; set; }

        public string ShortTermParkingAccessInfo { get; set; }

        public string ShortTermParkingDetails { get; set; }

        public string PropertyFeatures { get; set; }

        public int[] ShortTermPropertyImages { get; set; }

        public int? ShortTermParkingEntryTimeBufferMinutes { get; set; }

        public bool IsUncovered {
            get {
                return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("Uncovered");
            }
        }

        public bool IsCovered {
            get {
                return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("Covered");
            }
        }

        public bool IsElectric {
            get {
                return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("Electricity");
            }
        }

        public bool IsMetro {
            get {
                return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("Metro");
            }
        }

        public bool IsSecurity {
            get {
                return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("Security");
            }
        }

		public bool IsCctv {
			get {
				return !string.IsNullOrEmpty(this.PropertyFeatures) && this.PropertyFeatures.Contains("CCTV");
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

                    if (this.PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("Discount"));
                        PropertyChanged(this, new PropertyChangedEventArgs("TotalPrice"));
                    }
                }
            }
        }

        public decimal TotalPrice {
            get {
                if(this.Hours >= 8){
                    return this.ShortTermParkingFullDayPrice - this.Discount;
                }

                return (this.Hours * this.ShortTermParkingPrice) - this.Discount;
            }
        }

        public DateTime StartDate { get; set; }

        public int Hours { get; set; }

        public DateTime EndDate {
            get {
                return this.StartDate.AddHours(this.Hours);
            }
        }

        public string BookingTime {
            get {
                if (this.StartDate.Date == DateTime.Now.Date) {
                    return String.Format("{0} {1}", "Today", "Hourly");
                } else {
                    return String.Join(" to ", String.Format("{0} {1} {2:MMM HH:mm}", this.StartDate.ToString("ddd"), this.StartDate.Day.Ordinalize(), this.StartDate),
                                       String.Format("{0:HH:mm}", this.EndDate));
                }
            }
        }

		public string TotalTime {
			get {
                if (this.Hours >= 1 && this.StartDate.Date == DateTime.Now.Date) {
                    return "Today All Day (Until Midnight)";
                } else if (this.Hours >= 1 && this.StartDate.Date != DateTime.Now.Date) {
                    return String.Format("{0} {1} {2:MMM} All Day (Until Midnight)", this.StartDate.ToString("ddd"), this.StartDate.Day.Ordinalize(), this.StartDate);
                } else {
					return this.BookingTime;
				}
			}
		}

		public string ParkLaterBookingTime {
			get {
				if (this.StartDate.Date == DateTime.Now.Date) {
					return String.Join(" to ", String.Format("{0} {1: HH:mm}", "Today", this.StartDate), String.Format("{0: HH:mm}", this.EndDate));
				} else {
					return String.Join(" to ", String.Format("{0} {1} {2:MMM HH:mm}", this.StartDate.ToString("ddd"), this.StartDate.Day.Ordinalize(), this.StartDate),
									   String.Format("{0:HH:mm}", this.EndDate));
				}
			}
		}

		public string ParkLaterTotalTime {
			get {
				double parkinghours = this.Hours;

				var hours = (int)Math.Ceiling(parkinghours);

				var totalPrice = this.ShortTermParkingPrice * (decimal)hours;

				var fullDayPrice = this.ShortTermParkingFullDayPrice;

                if (this.Hours >= 8 || totalPrice >= fullDayPrice) {
					return String.Format("{0} {1} {2:MMM} All Day (Until Midnight)", this.StartDate.ToString("ddd"), this.StartDate.Day.Ordinalize(), this.StartDate);
				} else {
					return this.ParkLaterBookingTime;
				}
			}
		}

        public ImageSource Image {
            get {
                return ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/image/" + this.PropertyId));

            }
        }

		public List<KeyValuePair<int,ImageSource>> Images {
			get {
                return this.ShortTermPropertyImages!=null && this.ShortTermPropertyImages.Any() ? 
                           this.ShortTermPropertyImages
                           .Select(i => new KeyValuePair<int, ImageSource>(i, ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/getImage/" + i))))
                           .ToList() : null;

			}
		}

        public int EntryBufferMinutes { get; set; }

    }
}

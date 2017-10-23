﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Humanizer;

namespace YallaParkingMobile.Model {

    public class PropertyModel:INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public int PropertyId { get; set; }

        public string AreaName { get; set; }

        public string Name { get; set; }

        public string PropertyArea { get; set; }

        public string City { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public decimal ShortTermParkingPrice { get; set; }

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
					return String.Join(" to ", String.Format("{0} {1: HH:mm}", "Today", this.StartDate), String.Format("{0: HH:mm}", this.EndDate));
				} else {
					return String.Join(" to ", String.Format("{0} {1} {2:MMM HH:mm}", this.StartDate.ToString("ddd"), this.StartDate.Day.Ordinalize(), this.StartDate),
									   String.Format("{0} {1:MMM HH:mm}", this.EndDate.Day.Ordinalize(), this.EndDate));
				}
			}
		}

		public ImageSource Image {
			get {
				return ImageSource.FromUri(new Uri("http://yallaparking-new.insiso.co.uk/property/image/" + this.PropertyId));

			}
		}

        public int EntryBufferMinutes { get; set; }

    }
}

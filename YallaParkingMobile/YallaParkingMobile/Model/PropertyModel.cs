using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return string.Format("{0} to {1}", this.StartDate.ToString("dd-MMM-yy HH:mm"), this.EndDate.ToString("dd-MMM-yy HH:mm"));
            }
        }

        public string Image { get; set; }

        public string ImageBase {
            get {
                return !string.IsNullOrWhiteSpace(this.Image) ? this.Image.Split(',')[1] : this.Image;
            }
        }

    }
}

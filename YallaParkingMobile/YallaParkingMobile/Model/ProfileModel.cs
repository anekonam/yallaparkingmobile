using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaParkingMobile.Model {
    public class ProfileModel: INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        private string emiratesId { get; set; }
        public string EmiratesId {
            get {
                return emiratesId;
            }
            set {
                if (emiratesId != value) {
                    emiratesId = value;

                    if (this.PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("EmiratesId"));
                        PropertyChanged(this, new PropertyChangedEventArgs("Verified"));
                    }
                }
            }
        }

        public bool Verified {
            get {
                return !string.IsNullOrWhiteSpace(this.EmiratesId);
            }
        }

        public string Photo { get; set; }

        public string PhotoBase {
            get {
                return !string.IsNullOrWhiteSpace(this.Photo) && this.Photo.Contains(",") ? this.Photo.Split(',')[1] : this.Photo;
            }
        }
    }
}

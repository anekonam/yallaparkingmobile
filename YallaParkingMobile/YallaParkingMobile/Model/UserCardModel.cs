﻿using System;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Telerik.XamarinForms.Common.DataAnnotations;
using System.ComponentModel;

namespace YallaParkingMobile.Model {
    public class UserCardModel:INotifyPropertyChanged {
        
		public int? UserCardId { get; set; }

        public bool IsNew{
            get{
                return !this.UserCardId.HasValue;
            }
        }

		public bool IsNotNew {
			get {
				return this.UserCardId.HasValue;
			}
		}

		public ImageSource Image {
			get {
				if (!string.IsNullOrWhiteSpace(this.Brand)) {
					return ImageSource.FromFile(string.Format("{0}-logo.png", this.Brand.Replace(" ", "-")));
				}

				return null;
			}
		}

		public string Name { get; set; }

		public string Brand { get; set; }

		public string ExpireMonth { get; set; }

		public string ExpireYear { get; set; }

        public string ExpireMonthYear{
            get{
                return string.Format("{0}/{1}", this.ExpireMonth, this.ExpireYear);
            }
        }

        public string Number { get; set; }

        public string Cvc { get; set; }

		public string LastFourDigits { get; set; }


        public string EncodedCardNumber{
            get{
                return string.Format("**** **** **** {0}", this.LastFourDigits).Replace('*', '\u2022');
            }
        }

        private bool isSelected;
        public bool IsSelected{
            get{
                return isSelected;
            } set{
                if(isSelected!=value){
                    isSelected = value;

                    if(PropertyChanged!=null){
                        PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                    }

                }
            }
        }

		public event PropertyChangedEventHandler PropertyChanged;

	}
}

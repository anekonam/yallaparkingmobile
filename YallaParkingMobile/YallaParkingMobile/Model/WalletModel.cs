using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
namespace YallaParkingMobile.Model {
	public class WalletModel : INotifyPropertyChanged {

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

		public bool HasCards {
			get {
				return this.UserCards != null && this.UserCards.Any();
			}
		}

		public bool HasNoCards {
			get {
				return !this.HasCards;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

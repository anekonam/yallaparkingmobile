using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YallaParkingMobile.Utility {
    public static class PropertyUtility {

        public static string GetValue(string key) {
            return GetObjectValue(key) != null ? GetObjectValue(key).ToString() : null;
        }

        public static void SetValue(string key, string value) {
            Application.Current.Properties[key] = value;            
        }

        private static object GetObjectValue(string key) {
            if (Application.Current.Properties.ContainsKey(key)) {
                return Application.Current.Properties[key];
            }

            return null;
        }
    }
}

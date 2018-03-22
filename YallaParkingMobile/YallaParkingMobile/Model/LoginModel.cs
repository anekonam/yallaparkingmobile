using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace YallaParkingMobile.Model {
    //[PreserveAttribute(AllMembers = true)]
    [JsonObject(MemberSerialization.OptIn)]
    public class LoginModel {       

        public string EmailAddress { get; set; }

        public string Password { get; set; }                
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaParkingMobile.Model {
    public class TokenModel {

        public string Access_Token { get; set; }

        public string Token_Type { get; set; }

        public int Expire_In { get; set; }
    }
}

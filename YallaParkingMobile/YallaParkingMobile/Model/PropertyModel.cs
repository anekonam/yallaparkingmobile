using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YallaParkingMobile.Model {

    public class PropertyModel {
        public int PropertyId { get; set; }

        public string AreaName { get; set; }

        public string Name { get; set; }

        public string PropertyArea { get; set; }

        public string City { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public decimal ShortTermParkingPrice { get; set; }

    }
}

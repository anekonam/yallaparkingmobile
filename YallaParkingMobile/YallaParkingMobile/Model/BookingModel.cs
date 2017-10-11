using System;
namespace YallaParkingMobile.Model {
    public class BookingModel {
        
        public int UserCarId { get; set; }

        public int PropertyId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public double? Hours { get; set; }
    }
}

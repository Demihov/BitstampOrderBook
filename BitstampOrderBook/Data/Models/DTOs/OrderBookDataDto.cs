namespace BitstampOrderBook.Data.Models.DTOs
{
    public class OrderBookDataDto
    {
        public double Timestamp { get; set; }
        public double Microtimestamp { get; set; }
        public List<List<decimal>> Bids { get; set; }
        public List<List<decimal>> Asks { get; set; }
    }
}

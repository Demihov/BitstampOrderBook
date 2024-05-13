namespace BitstampOrderBook.Data.Models
{
    public class OrderBook
    {
        public int Id { get; set; }
        public double Timestamp { get; set; }
        public double MicroTimestamp { get; set; }

        public List<Order> Bids { get; set; } = new List<Order>();
        public List<Order> Asks { get; set; } = new List<Order>();
    }
}

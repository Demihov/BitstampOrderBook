namespace BitstampOrderBook.Data.Models
{
    public class OrderBook
    {
        public int Id { get; set; }
        public double Timestamp { get; set; }
        public double MicroTimestamp { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}

namespace BitstampOrderBook.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        public int OrderBookId { get; set; }
        public OrderBook OrderBook { get; set; }
    }
}

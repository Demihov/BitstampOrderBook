﻿namespace BitstampOrderBook.Data.Models
{
    public enum OrderType
    {
        Bid,
        Ask
    }

    public class Order
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public OrderType OrderType { get; set; }

        public int OrderBookId { get; set; }
        public OrderBook OrderBook { get; set; }
    }
}

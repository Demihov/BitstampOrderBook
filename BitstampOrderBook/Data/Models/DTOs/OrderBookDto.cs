﻿namespace BitstampOrderBook.Data.Models.DTOs
{
    public class OrderBookDto
    {
        public OrderBookDataDto Data { get; set; }
        public string Channel { get; set; }
        public string Event { get; set; }
    }
}

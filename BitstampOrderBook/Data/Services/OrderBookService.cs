using BitstampOrderBook.Data.Models;
using Newtonsoft.Json;

namespace BitstampOrderBook.Data.Services
{
    public class OrderBookService
    {
        private readonly ApplicationDbContext _context;

        public OrderBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveOrderBookAsync(string jsonOrderBook)
        {
            try
            {
                var orderBookDto = JsonConvert.DeserializeObject<OrderBookDto>(jsonOrderBook);
                var orderBook = new OrderBook
                {
                    Timestamp = orderBookDto.Data.Timestamp,
                    MicroTimestamp = orderBookDto.Data.Microtimestamp
                };

                _context.OrderBooks.Add(orderBook);
                await _context.SaveChangesAsync();


                foreach (var bid in orderBookDto.Data.Bids)
                {
                    var order = new Order
                    {
                        OrderBookId = orderBook.Id,
                        Price = bid[0],
                        Amount = bid[1]
                    };
                    _context.Orders.Add(order);
                }

                foreach (var ask in orderBookDto.Data.Asks)
                {
                    var order = new Order
                    {
                        OrderBookId = orderBook.Id,
                        Price = ask[0],
                        Amount = ask[1]
                    };
                    _context.Orders.Add(order);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                await Console.Out.WriteLineAsync();
            }
        }

        public class OrderBookDto
        {
            public OrderBookData Data { get; set; }
            public string Channel { get; set; }
            public string Event { get; set; }
        }

        public class OrderBookData
        {
            public double Timestamp { get; set; }
            public double Microtimestamp { get; set; }
            public List<List<decimal>> Bids { get; set; }
            public List<List<decimal>> Asks { get; set; }
        }
    }
}

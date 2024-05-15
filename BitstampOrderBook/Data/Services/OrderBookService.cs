using BitstampOrderBook.Data.Models;
using BitstampOrderBook.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BitstampOrderBook.Data.Services
{
    public class OrderBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderBookService> _logger;

        public OrderBookService(ApplicationDbContext context, ILogger<OrderBookService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveOrderBookAsync(OrderBookDto orderBookDto)
        {
            if (orderBookDto == null) return;
            try
            {
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
                        Amount = bid[1],
                        OrderType = OrderType.Bid
                    };
                    _context.Orders.Add(order);
                }

                foreach (var ask in orderBookDto.Data.Asks)
                {
                    var order = new Order
                    {
                        OrderBookId = orderBook.Id,
                        Price = ask[0],
                        Amount = ask[1],
                        OrderType = OrderType.Ask
                    };
                    _context.Orders.Add(order);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving order book");
            }
        }

        public async Task<OrderBookDto> GetOrderBookByTimestampAsync(double timestamp)
        {
            var orderBook = await _context.OrderBooks
                .Where(ob => ob.MicroTimestamp == timestamp)
                .Include(ob => ob.Orders)
                .FirstOrDefaultAsync();

            if (orderBook == null)
            {
                return null;
            }

            var bids = orderBook.Orders
                .Where(o => o.OrderType == OrderType.Bid)
                .Select(o => new List<decimal> { o.Price, o.Amount })
                .ToList();

            var asks = orderBook.Orders
                .Where(o => o.OrderType == OrderType.Ask)
                .Select(o => new List<decimal> { o.Price, o.Amount })
                .ToList();

            var orderBookDto = new OrderBookDto
            {
                Data = new OrderBookDataDto
                {
                    Timestamp = orderBook.Timestamp,
                    Microtimestamp = orderBook.MicroTimestamp,
                    Bids = bids,
                    Asks = asks
                }
            };

            return orderBookDto;
        }

        public async Task<decimal> GetBTCPriceByAmountAsync(decimal amount)
        {
            var lastOrderBook = await _context.OrderBooks
                .OrderByDescending(ob => ob.MicroTimestamp)
                .Include(ob => ob.Orders)
                .FirstOrDefaultAsync();

            var bids = lastOrderBook.Orders
               .Where(o => o.OrderType == OrderType.Bid)
               .OrderBy(o => o.Price)
               .Select(o => (o.Price, o.Amount))
               .ToList();

            if (bids == null || !bids.Any())
            {
                return 0;
            }

            decimal price = 0;

            foreach (var bid in bids)
            {
                if (amount - bid.Amount >= 0)
                {
                    price += bid.Amount * bid.Price;
                    amount -= bid.Amount;
                }
                else
                {
                    price += amount * bid.Price;
                    break;
                }
            }

            return price;
        }
    }
}

using BitstampOrderBook.Data;
using BitstampOrderBook.Data.Models;
using BitstampOrderBook.Data.Models.DTOs;
using BitstampOrderBook.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BitstampOrderBook.UnitTests.Services
{
    [TestFixture]
    public class OrderBookServiceTests
    {
        private ApplicationDbContext _context;
        private OrderBookService _orderBookService;
        private Mock<ILogger<OrderBookService>> _logger;

        private OrderBookDto orderBookDto;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemory")
                .Options;

            _context = new ApplicationDbContext(options);
            _logger = new Mock<ILogger<OrderBookService>>();
            _orderBookService = new OrderBookService(_context, _logger.Object);

            orderBookDto = new OrderBookDto
            {
                Data = new OrderBookDataDto
                {
                    Timestamp = 1715806644,
                    Microtimestamp = 1715806644145014,
                    Bids = new List<List<decimal>>
                    {
                        new List<decimal> { 65000, 0.2m },
                        new List<decimal> { 65500, 0.4m }
                    },
                    Asks = new List<List<decimal>>
                    {
                        new List<decimal> { 66000, 0.3m },
                        new List<decimal> { 66500, 0.4m }
                    }
                }
            };

            await _orderBookService.SaveOrderBookAsync(orderBookDto);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void SaveOrderBookAsync_WithValidOrderBookDto_ShouldSaveOrderBook()
        {
            var orderBook = _context.OrderBooks.Include(ob => ob.Orders).First();
            Assert.IsNotNull(orderBook);
            Assert.AreEqual(orderBookDto.Data.Timestamp, orderBook.Timestamp);
            Assert.AreEqual(orderBookDto.Data.Microtimestamp, orderBook.MicroTimestamp);

            var orders = orderBook.Orders.ToList();

            Assert.AreEqual(4, orders.Count);

            Assert.AreEqual(OrderType.Bid, orders[0].OrderType);
            Assert.AreEqual(65000, orders[0].Price);
            Assert.AreEqual(0.2m, orders[0].Amount);

            Assert.AreEqual(OrderType.Bid, orders[1].OrderType);
            Assert.AreEqual(65500, orders[1].Price);
            Assert.AreEqual(0.4m, orders[1].Amount);

            Assert.AreEqual(OrderType.Ask, orders[2].OrderType);
            Assert.AreEqual(66000, orders[2].Price);
            Assert.AreEqual(0.3m, orders[2].Amount);

            Assert.AreEqual(OrderType.Ask, orders[3].OrderType);
            Assert.AreEqual(66500, orders[3].Price);
            Assert.AreEqual(0.4m, orders[3].Amount);
        }


        [Test]
        public async Task GetOrderBookByTimestampAsync_WithValidTimestamp_ShouldReturnOrderBookDto()
        {
            var result = await _orderBookService.GetOrderBookByTimestampAsync(orderBookDto.Data.Microtimestamp);

            Assert.IsNotNull(result);
            Assert.AreEqual(orderBookDto.Data.Timestamp, result.Data.Timestamp);
            Assert.AreEqual(orderBookDto.Data.Microtimestamp, result.Data.Microtimestamp);
            Assert.AreEqual(2, result.Data.Bids.Count);
            Assert.AreEqual(2, result.Data.Asks.Count);
        }

        [Test]
        public void GetOrderBookByTimestampAsync_WithInvalidTimestamp_ShouldReturnNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _orderBookService.GetOrderBookByTimestampAsync(-1));

            Assert.AreEqual("Invalid timestamp value. (Parameter 'timestamp')", exception.Message);
            Assert.AreEqual("timestamp", exception.ParamName);
        }


        [Test]
        public async Task GetBTCPriceByAmountAsync_WithValidAmount_ShouldReturnCorrectPrice()
        {
            var price = await _orderBookService.GetBTCPriceByAmountAsync(0.5m);
            Assert.AreEqual(65000 * 0.2m + 65500 * 0.3m, price);
        }

        [Test]
        public void GetBTCPriceByAmountAsync_WithInvalidAmount_ShouldReturnZero()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _orderBookService.GetBTCPriceByAmountAsync(-1));

            Assert.AreEqual("Invalid amount value. (Parameter 'amount')", exception.Message);
            Assert.AreEqual("amount", exception.ParamName);
        }
    }
}

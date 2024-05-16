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

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemory")
                .Options;

            _context = new ApplicationDbContext(options);
            _logger = new Mock<ILogger<OrderBookService>>();
            _orderBookService = new OrderBookService(_context, _logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task SaveOrderBookAsync_WithValidOrderBookDto_ShouldSaveOrderBook()
        {
            // Arrange
            var orderBookDto = new OrderBookDto
            {
                Data = new OrderBookDataDto
                {
                    Timestamp = 1715806644,
                    Microtimestamp = 1715806644145014,
                    Bids = new List<List<decimal>>
                    {
                        new List<decimal> { 100, 1 },
                        new List<decimal> { 101, 2 }
                    },
                    Asks = new List<List<decimal>>
                    {
                        new List<decimal> { 200, 3 },
                        new List<decimal> { 201, 4 }
                    }
                }
            };

            // Act
            await _orderBookService.SaveOrderBookAsync(orderBookDto);

            // Assert
            var orderBook = _context.OrderBooks.Include(ob => ob.Orders).First();
            Assert.IsNotNull(orderBook);
            Assert.AreEqual(orderBookDto.Data.Timestamp, orderBook.Timestamp);
            Assert.AreEqual(orderBookDto.Data.Microtimestamp, orderBook.MicroTimestamp);

            var orders = orderBook.Orders.ToList();

            Assert.AreEqual(4, orders.Count);

            Assert.AreEqual(OrderType.Bid, orders[0].OrderType);
            Assert.AreEqual(100, orders[0].Price);
            Assert.AreEqual(1, orders[0].Amount);

            Assert.AreEqual(OrderType.Bid, orders[1].OrderType);
            Assert.AreEqual(101, orders[1].Price);
            Assert.AreEqual(2, orders[1].Amount);

            Assert.AreEqual(OrderType.Ask, orders[2].OrderType);
            Assert.AreEqual(200, orders[2].Price);
            Assert.AreEqual(3, orders[2].Amount);

            Assert.AreEqual(OrderType.Ask, orders[3].OrderType);
            Assert.AreEqual(201, orders[3].Price);
            Assert.AreEqual(4, orders[3].Amount);
        }
    }
}

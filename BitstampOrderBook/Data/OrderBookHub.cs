using BitstampOrderBook.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace BitstampOrderBook.Data
{
    public class OrderBookHub : Hub
    {
        private readonly OrderBookService _orderBookService;

        public OrderBookHub(OrderBookService orderBookService)
        {
            _orderBookService = orderBookService;
        }

        public async Task UpdateBTCPrice(decimal amount)
        {
            var btcPrice = await _orderBookService.GetBTCPriceByAmountAsync(amount);
            await Clients.All.SendAsync("ReceiveBTCPrice", btcPrice);
        }
    }
}

using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace BitstampOrderBook.Data.Services
{
    public class WebSocketService
    {
        private readonly ClientWebSocket _clientWebSocket;
        private readonly ILogger<WebSocketService> _logger;
        private readonly OrderBookService _orderBookService;

        public WebSocketService(OrderBookService orderBookService, ILogger<WebSocketService> logger)
        {
            _clientWebSocket = new ClientWebSocket();
            _logger = logger;
            _orderBookService = orderBookService;
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await _clientWebSocket.ConnectAsync(uri, cancellationToken);
            _logger.LogInformation("WebSocket connected to {Uri}", uri);
            await SendSubscriptionMessageAsync(cancellationToken);
            await ReceiveMessagesAsync(cancellationToken);
        }

        private async Task SendSubscriptionMessageAsync(CancellationToken cancellationToken)
        {
            var subscribeMessage = new
            {
                @event = "bts:subscribe",
                data = new
                {
                    channel = "order_book_btceur"
                }
            };
            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(subscribeMessage));
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 8];
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                    _logger.LogInformation("WebSocket closed");
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await HandleMessageAsync(message);
                }
            }
        }

        private async Task HandleMessageAsync(string message)
        {
            try
            {
                await _orderBookService.SaveOrderBookAsync(message);
                _logger.LogInformation("Order book updated from WebSocket message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling WebSocket message.");
            }
        }
    }
}

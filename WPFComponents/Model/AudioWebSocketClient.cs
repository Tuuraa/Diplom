using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using Windows.Networking.Sockets;

/// <summary>
/// Класс для общения с Python и мобильным приложением через WebSocket
/// </summary>
class WebSocketServer
{
    private HttpListener _httpListener;
    public event Action<string> OnTextReceived;
    private ConcurrentBag<WebSocket> _clients = new ConcurrentBag<WebSocket>();

    public async Task StartAsync(string url)
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(url);
        //_httpListener.Start();

        Console.WriteLine("WebSocket сервер запущен.");

        //while (true)
        //{
        //    var context = await _httpListener.GetContextAsync();
        //    if (context.Request.IsWebSocketRequest)
        //    {
        //        try
        //        {
        //            var webSocketContext = await context.AcceptWebSocketAsync(null);
        //            var webSocket = webSocketContext.WebSocket;

        //            _clients.Add(webSocket);
        //            Console.WriteLine($"Новое WebSocket-подключение. Всего подключений: {_clients.Count}");

        //            _ = HandleWebSocketConnection(webSocket);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Ошибка при установке соединения: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        context.Response.StatusCode = 400;
        //        context.Response.Close();
        //    }
        //}
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Получено сообщение: {message}");

                    OnTextReceived?.Invoke(message);
                    await BroadcastAsync($"Echo: {message}");
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Клиент отключился.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Отключение", CancellationToken.None);
                    _clients = new ConcurrentBag<WebSocket>(_clients.Except(new[] { webSocket }));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка в WebSocket: " + ex.Message);
        }
    }

    /// <summary>
    /// Отправляет сообщение конкретному клиенту
    /// </summary>
    public async Task SendAsync(WebSocket client, string message)
    {
        if (client == null || client.State != WebSocketState.Open)
        {
            Console.WriteLine("Ошибка: клиент недоступен.");
            return;
        }

        var buffer = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    /// <summary>
    /// Отправляет сообщение всем клиентам
    /// </summary>
    public async Task BroadcastAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in _clients)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public async Task BroadcastAsync(byte[] data)
    {
        foreach (var client in _clients)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
    }

    public void Stop()
    {
        _httpListener.Stop();
        Console.WriteLine("WebSocket сервер остановлен.");
    }
}

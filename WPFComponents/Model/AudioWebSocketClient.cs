using System.Net;
using System.Net.WebSockets;
using System.Text;

/// <summary>
/// Класс для общения с питононом для распознования речи
/// </summary>
class WebSocketServer
{
    private HttpListener _httpListener;
    public event Action<string> OnTextReceived;

    public async Task StartAsync(string url)
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(url);
        _httpListener.Start();


        while (true)
        {
            var context = await _httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                WebSocket webSocket = null;
                try
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    webSocket = webSocketContext.WebSocket;

                    Console.WriteLine("WebSocket connection established.");
                    await HandleWebSocketConnection(webSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024];
        WebSocketReceiveResult result;

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    OnTextReceived?.Invoke(message);

                    var response = Encoding.UTF8.GetBytes($"Echo: {message}");
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    Console.WriteLine("Connection closed.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void Stop()
    {
        _httpListener.Stop();
    }

}

using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Ждем пока сервер откроет порт
        Console.WriteLine("Waiting for WebSocket server...");
        while (!IsServerAvailable("localhost", 5001))
        {
            await Task.Delay(1000);
            Console.Write(".");
        }

        Console.WriteLine("\nServer is ready!");

        using var client = new ClientWebSocket();
        var serverUri = new Uri("ws://localhost:5001/");

        try
        {
            await client.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server.");

            _ = Task.Run(async () => await ReceiveMessages(client));

            while (client.State == WebSocketState.Open)
            {
                Console.Write("Enter message: ");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                    continue;

                var bytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            if (client.State == WebSocketState.Open)
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
    }

    static bool IsServerAvailable(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            return client.ConnectAsync(host, port).Wait(500);
        }
        catch
        {
            return false;
        }
    }

    static async Task ReceiveMessages(ClientWebSocket client)
    {
        var buffer = new byte[1024];
        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server closed connection.");
                break;
            }
        }
    }
}
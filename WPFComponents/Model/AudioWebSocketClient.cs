using NAudio.Wave;
using System.Net.WebSockets;
using System.Text;
using System.Windows;

namespace WPFComponents.Model
{
    public class AudioWebSocketClient
    {
        private readonly Uri serverUri;
        private ClientWebSocket webSocket;
        private WaveInEvent waveIn;
        private const int BufferSize = 4096;

        public event Action<string> OnPartialTextReceived;

        public AudioWebSocketClient(string serverUrl)
        {
            serverUri = new Uri(serverUrl);
            webSocket = new ClientWebSocket();
        }
        public async Task ConnectAsync()
        {
            try
            {
                await webSocket.ConnectAsync(serverUri, CancellationToken.None);
                MessageBox.Show("WebSocket подключен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }

        public async Task StartRecognitionAsync()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                byte[] startMessage = Encoding.UTF8.GetBytes("start");
                await webSocket.SendAsync(new ArraySegment<byte>(startMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                StartRecording();
            }
        }

        private void StartRecording()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000, 1); 
            waveIn.DataAvailable += async (sender, e) =>
            {
                await SendAudioDataAsync(e.Buffer, e.BytesRecorded);
            };
            waveIn.StartRecording();
        }

        private async Task SendAudioDataAsync(byte[] buffer, int bytesRecorded)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRecorded), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }
        public async Task ReceiveRecognitionResultAsync()
        {
            var buffer = new byte[BufferSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // Генерация события при получении частичного текста
                    OnPartialTextReceived?.Invoke(receivedText);
                }
            }
        }
        public async Task DisconnectAsync()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие", CancellationToken.None);
                webSocket.Dispose();
                waveIn?.Dispose();
            }
        }
    }
}

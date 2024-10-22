using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class AudioWebSocketClient
    {
        private readonly Uri serverUri;
        private ClientWebSocket webSocket;
        private WaveInEvent waveIn;
        private const int BufferSize = 4096;
        private Stopwatch stopwatch = new Stopwatch();
        private const float SilenceThreshold = -10.0f; // Уровень громкости для определения тишины
        private int _index = 0;

        // Событие для уведомления об обнаружении тишины
        public event EventHandler SilenceDetected;

        // Событие для получения частичных результатов распознавания
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
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
            waveIn.WaveFormat = new WaveFormat(16000, 1); // 16 кГц, моно

            waveIn.DataAvailable += async (sender, e) =>
            {
                float volume = CalculateVolume(e.Buffer, e.BytesRecorded);


                await SendAudioDataAsync(e.Buffer, e.BytesRecorded);
            };

            waveIn.StartRecording();
        }

        private float CalculateVolume(byte[] buffer, int bytesRecorded)
        {
            int bytesPerSample = 2; // 16 бит = 2 байта на один сэмпл
            int sampleCount = bytesRecorded / bytesPerSample;
            float sum = 0;

            for (int i = 0; i < bytesRecorded; i += bytesPerSample)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                float sample32 = sample / 32768f; // Преобразуем в диапазон от -1 до 1
                sum += sample32 * sample32;
            }

            float rms = (float)Math.Sqrt(sum / sampleCount);
            float volumeDb = 20 * (float)Math.Log10(rms);

            return volumeDb;
        }

        private async Task HandleSilenceAsync()
        {
            // Останавливаем запись
            waveIn.StopRecording();

            // Закрываем WebSocket соединение
            await DisconnectAsync();
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
                    string receivedText = Encoding.UTF8.GetString(buffer, _index, result.Count);
                    _index += result.Count; // Сброс индекса после получения полного сообщения

                    // Вызываем событие при получении текста
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

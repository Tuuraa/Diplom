using NAudio.Wave;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFComponents.Utils
{
    internal class SoundWave
    {
        private WaveInEvent waveIn;
        private List<double> samples = new List<double>();

        private const int SampleRate = 44100;
        private const double SensitivityFactor = 1.5;
        private const double HeightMultiplier = 2.0;

        private Canvas soundCanvas;
        private Polyline waveLine;

        public SoundWave(Canvas canvas, Polyline waveline)
        {
            soundCanvas = canvas;
            this.waveLine = waveline;

            StartMicrophone();
            CompositionTarget.Rendering += OnRender;
        }

        private void StartMicrophone()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(SampleRate, 1); // Моно
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            // Преобразование байтов в амплитуды
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                // Получаем амплитуду (16-битное значение)
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                double normalizedSample = sample / 32768.0; // Нормализация
                samples.Add(normalizedSample);
            }
        }

        private void OnRender(object sender, EventArgs e)
        {
            if (samples.Count == 0) return;

            int width = (int)soundCanvas.ActualWidth;
            double height = soundCanvas.ActualHeight;

            // Подготовка точек для визуализации
            PointCollection points = new PointCollection();
            for (int i = 0; i < width; i++)
            {
                int index = (int)((i / (double)width) * samples.Count);
                double y = (height / 2) + (samples[index] * (height / 2) * SensitivityFactor * HeightMultiplier); // Увеличение высоты
                points.Add(new Point(i, y));
            }

            waveLine.Points = points;

            // Очищаем старые значения для следующего обновления
            samples.Clear();
        }
    }
}

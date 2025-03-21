using CommunityToolkit.Mvvm.ComponentModel;
using QRCoder;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace WPFComponents.ViewModel
{
    public partial class PhoneConnectWindowVM : ObservableObject
    {
        private const string DefaultQrText = "ws://localhost:5001";
        private const string LogoPath = "C:\\DiplomUI\\WPFComponents\\Media\\Icons\\wave.ico";

        [ObservableProperty]
        private BitmapImage qrCodeImage;

        private GradientModel _selectedGradient;
        public GradientModel SelectedGradient
        {
            get => _selectedGradient;
            set
            {
                SetProperty(ref _selectedGradient, value);
                GenerateQrCode(); // Вызов генерации при изменении
            }
        }

        public ObservableCollection<GradientModel> GradientOptions { get; } = new ObservableCollection<GradientModel>
        {
            new GradientModel("#FF0000", "#0000FF"), // Красно-синий
            new GradientModel("#FF4500", "#FFD700"), // Оранжево-желтый
            new GradientModel("#9400D3", "#4B0082"), // Фиолетовый градиент
            new GradientModel("#008000", "#00FF00"), // Зелёный
            new GradientModel("#000000", "#808080"), // Серый
            new GradientModel("#FF1493", "#FF69B4"), // Розовый
            new GradientModel("#00FFFF", "#1E90FF")  // Голубой
        };

        public PhoneConnectWindowVM()
        {
            SelectedGradient = GradientOptions.First(); // Устанавливаем первый градиент
            GenerateQrCode(); // Генерация QR-кода при старте
        }

        public void GenerateQrCode()
        {
            if (SelectedGradient == null) return;
            QrCodeImage = null; // Сбрасываем старый QR-код

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(DefaultQrText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);

            Color drawDark = ColorTranslator.FromHtml(SelectedGradient.Color1);
            Color drawLight = ColorTranslator.FromHtml(SelectedGradient.Color2);

            var qrCodeBitmap = qrCode.GetGraphic(20, drawDark, drawLight, true);

            // Добавление логотипа
            if (File.Exists(LogoPath))
            {
                using (var logo = (Bitmap)Image.FromFile(LogoPath))
                {
                    int overlaySize = qrCodeBitmap.Width / 5;
                    using (Graphics g = Graphics.FromImage(qrCodeBitmap))
                    {
                        g.DrawImage(logo, (qrCodeBitmap.Width - overlaySize) / 2,
                                          (qrCodeBitmap.Height - overlaySize) / 2,
                                          overlaySize, overlaySize);
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                QrCodeImage = bitmapImage;
            }
        }
    }

    public class GradientModel
    {
        public string Color1 { get; }
        public string Color2 { get; }
        public string Display => $"{Color1} → {Color2}";

        public GradientModel(string color1, string color2)
        {
            Color1 = color1;
            Color2 = color2;
        }
    }
}

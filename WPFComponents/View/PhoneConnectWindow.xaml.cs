using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFComponents.View
{
    public partial class PhoneConnectWindow : Window
    {
        public PhoneConnectWindow()
        {
            InitializeComponent();

            string qrText = "ws://localhost:5001";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData); // Используем QRCode вместо ArtQRCode

            // Настройка цветов
            var drawDark = (Color.FromArgb(0, 136, 204)); // Цвет Telegram
            var drawLight = (Color.White);

            // Генерация QR-кода с квадратными точками
            var qrCodeBitmap = qrCode.GetGraphic(20, drawDark, drawLight, true);

            // Добавление логотипа с белым фоном
            var logo = (Bitmap)Image.FromFile("C:\\DiplomUI\\WPFComponents\\Media\\Icons\\wave.ico");
            qrCodeBitmap = AddLogoToQrCode(qrCodeBitmap, logo, 15);

            // Преобразование Bitmap в BitmapImage для использования в WPF
            var ms = new MemoryStream();
            qrCodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            // Отображение QR-кода в Image
            QRCodeImage.Source = bitmapImage;
        }

        private Bitmap AddLogoToQrCode(Bitmap qrCodeBitmap, Bitmap logo, int iconSizePercent)
        {
            int iconSize = Math.Min(qrCodeBitmap.Width * iconSizePercent / 100, qrCodeBitmap.Height * iconSizePercent / 100);
            int iconX = (qrCodeBitmap.Width - iconSize) / 2;
            int iconY = (qrCodeBitmap.Height - iconSize) / 2;

            // Создаем белый фон для логотипа
            using (Graphics g = Graphics.FromImage(qrCodeBitmap))
            {
                // Рисуем белый прямоугольник под логотип
                g.FillRectangle(Brushes.White, new Rectangle(iconX, iconY, iconSize, iconSize));

                // Рисуем логотип поверх белого фона
                g.DrawImage(logo, new Rectangle(iconX, iconY, iconSize, iconSize));
            }

            return qrCodeBitmap;
        }
    }
}
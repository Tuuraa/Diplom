using System.Windows;
using H.NotifyIcon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WPFComponents.DB;
using WPFComponents.Model;
using WPFComponents.Services;

namespace WPFComponents
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Добавляем зависимости в DI-контейнер
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlite("Data Source=database.db"));

            services.AddTransient<LoggerService>();

            services.AddTransient<LLMActionService>();

            services.AddTransient<VoiceCommandProcessor>();

            // Регистрируем MainWindow
            services.AddTransient<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            // Получаем MainWindow через DI
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}

using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPFComponents.Services;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();


            serviceCollection.AddSingleton<LoggerService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();


            base.OnStartup(e);
        }

    }

}

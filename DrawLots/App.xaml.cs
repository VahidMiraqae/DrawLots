using System.Globalization;
using System.Windows;

namespace DrawLots
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CultureInfo.CurrentCulture = new CultureInfo("fa-ir");
            CultureInfo.CurrentUICulture = new CultureInfo("fa-IR");

            //    FrameworkElement.LanguageProperty.OverrideMetadata(
            //typeof(FrameworkElement),
            //new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var windowService = new WindowService();
            var context = new DrawingDataContext();
            var mainViewModel = new MainViewModel(windowService, context);
            MainWindow = new MainWindow(mainViewModel);
            MainWindow.Show();
        }
    }
}

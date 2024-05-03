using System.Windows;

namespace DrawLots
{
    public partial class MainWindow : Window
    {
        internal MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}

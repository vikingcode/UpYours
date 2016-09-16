using System.Windows;
using UpYours.ViewModel;

namespace UpYours
{
    public partial class MainWindow : Window
    {
        private MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as MainViewModel;
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            vm.OpenFile();
        }
    }
}

namespace Isolines3D.UI
{
    using Isolines3D.UI.ViewModels;
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM(mainView);
        }
    }
}

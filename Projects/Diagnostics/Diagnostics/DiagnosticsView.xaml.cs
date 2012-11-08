using System.Windows;


namespace Diagnostics
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DiagnosticsView : Window
    {
        public DiagnosticsView()
        {
            InitializeComponent();
            var diagnosticsViewModel = new DiagnosticsViewModel();
            this.DataContext = diagnosticsViewModel;
        }
    }
}

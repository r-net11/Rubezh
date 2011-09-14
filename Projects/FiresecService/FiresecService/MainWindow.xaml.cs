using System.Windows;
using FiresecService;
using FiresecService.DatabaseConverter;
using FiresecService.Imitator;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Current = this;
        }

        public static MainWindow Current { get; private set; }
        public static void AddMessage(string message)
        {
            Current._textBox.Text += message + "\n";
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            FiresecManager.ConnectFiresecCOMServer("adm", "");
            FiresecServiceManager.Open();
        }

        void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var imitatorView = new ImitatorView();
            imitatorView.Show();
        }

        readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();
        void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ConfigurationConverter.Convert();
            return;

            JournalDataConverter.Convert();
        }
    }
}
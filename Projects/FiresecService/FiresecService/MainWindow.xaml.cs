using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using FiresecAPI.Models;
using FiresecService;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;
using FiresecService.Views;

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
            FiresecManager.Connect("adm", "");
            FiresecServiceManager.Open();
        }

        void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var alarmTest = new AlarmTest();
            alarmTest.Show();
        }

        readonly static FiresecDbConverterDataContext DataBaseContext = new FiresecDbConverterDataContext();
        void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ConvertPlans();
            return;

            ConfigurationConverter.Convert();
            JournalDataConverter.Convert();
            JournalDataConverter.Select();
        }

        void ConvertPlans()
        {
            var plans = FiresecInternalClient.GetPlans();
            var plansConfiguration = PlansConverter.Convert(plans);

            var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
            using (var fileStream = new FileStream("Configuration/PlansConfiguration.xml", FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, plansConfiguration);
            }

            FiresecManager.PlansConfiguration = plansConfiguration;
        }
    }
}
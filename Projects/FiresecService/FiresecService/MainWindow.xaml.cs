using System.Windows;
using FiresecService;
using FiresecService.Views;
using FiresecService.Converters;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using System.IO;

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
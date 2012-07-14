using System.Configuration;
using System.Windows;
using FiresecClient;

namespace ItvIntegration
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }

		public MainViewModel()
		{
			string serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
			string defaultLogin = ConfigurationManager.AppSettings["DefaultLogin"] as string;
			string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"] as string;
			string result = ItvManager.Connect(serverAddress, defaultLogin, defaultPassword);
			if (result != null)
			{
				MessageBox.Show(result);
				return;
			}

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
		}
	}
}
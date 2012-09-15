using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.Reports
{
	internal class ElementPage
	{
		private ElementPage() { }

		public ElementPage(int number, Device device)
		{
			No = number;
			_device = device;
		}

		Device _device;
		public int No { get; set; }

		public string PresentationName
		{
			get
			{
				return FiresecManager.FiresecConfiguration.GetIndicatorString(_device);
			}
		}
	}
}
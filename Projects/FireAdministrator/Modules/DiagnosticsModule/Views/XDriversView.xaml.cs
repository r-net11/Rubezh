using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using XFiresecAPI;

namespace DiagnosticsModule.Views
{
	public partial class XDriversView : Window
	{
		public XDriversView()
		{
			InitializeComponent();

			Drivers = (from XDriver driver in XManager.Drivers
					   select driver).ToList();

			DataContext = this;
		}

		public List<XDriver> Drivers { get; private set; }
	}
}
using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace DiagnosticsModule.ViewModels
{
	public class PerformanceItemViewModel : TreeNodeViewModel<PerformanceItemViewModel>
	{
		public PerformanceItemViewModel(XDriver driver)
		{
			Driver = driver;
		}

		public XDriver Driver { get; private set; }
		public string PresentationAddress
		{
			get { return Driver.DriverType.ToString(); }
		}
		public string Description
		{
			get { return Driver.ShortName; }
		}
		public bool IsBold { get; set; }
	}
}
using Infrastructure.Common.Windows.ViewModels;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class LayoutViewModel : BaseViewModel
	{
		public LayoutModel Layout { get; private set; }

		public LayoutViewModel(LayoutModel layout)
		{
			Layout = layout;
		}

		public string Name
		{
			get { return Layout.Caption; }
		}
	}
}
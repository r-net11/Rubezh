using Infrastructure.Common.Windows.ViewModels;
using Common;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class LayoutPartPropertyViewModel : BaseViewModel
	{
		public LayoutPartProperty LayoutPartProperty { get; private set; }

		public LayoutPartPropertyViewModel(LayoutPartProperty layoutPartProperty)
		{
			LayoutPartProperty = layoutPartProperty;
		}

		public LayoutPartPropertyName Name
		{
			get { return LayoutPartProperty.Name; }
		}
		public LayoutPartPropertyType Type
		{
			get { return LayoutPartProperty.PropertyType; }
		}
	}
}
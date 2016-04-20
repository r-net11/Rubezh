using Infrastructure.Common.Windows.Windows.ViewModels;
using Common;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;
using Infrastructure.Common.Windows.Services.Layout;
using RubezhAPI.Automation;

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
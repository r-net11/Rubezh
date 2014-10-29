using Infrastructure.Common.Windows.ViewModels;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace AutomationModule.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel
	{
		public LayoutPart LayoutPart { get; private set; }
		public ILayoutPartDescription Description { get; private set; }

		public LayoutPartViewModel(LayoutPart layoutPart, ILayoutPartDescription description)
		{
			LayoutPart = layoutPart;
			Description = description;
		}

		public string Name
		{
			get { return LayoutPart.Title ?? (Description == null ? string.Empty : Description.Name); }
		}
	}
}
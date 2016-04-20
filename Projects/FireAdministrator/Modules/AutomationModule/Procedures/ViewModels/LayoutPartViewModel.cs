using Infrastructure.Common.Windows.Windows.ViewModels;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Services.Layout;

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
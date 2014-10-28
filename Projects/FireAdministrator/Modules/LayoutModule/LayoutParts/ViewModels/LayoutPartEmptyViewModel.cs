using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartEmptyViewModel : LayoutPartTitleViewModel
	{
		public LayoutPartEmptyViewModel()
		{
			Title = "Заглушка";
			IconSource = LayoutPartDescription.IconPath + "BExit.png";
		}
	}
}
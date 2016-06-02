using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using Localization.Layout.ViewModels;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartEmptyViewModel : LayoutPartTitleViewModel
	{
		public LayoutPartEmptyViewModel()
		{
			Title = CommonViewModels.LayoutPartEmpty_Title;
			IconSource = LayoutPartDescription.IconPath + "BExit.png";
		}
	}
}
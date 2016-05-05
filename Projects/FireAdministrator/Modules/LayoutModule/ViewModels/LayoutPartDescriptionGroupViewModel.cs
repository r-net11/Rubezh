using StrazhAPI;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.TreeList;

namespace LayoutModule.ViewModels
{
	public class LayoutPartDescriptionGroupViewModel : TreeNodeViewModel<LayoutPartDescriptionGroupViewModel>
	{
		public LayoutPartDescriptionGroupViewModel()
		{
		}
		public LayoutPartDescriptionGroupViewModel(LayoutPartDescriptionGroup group)
		{
			GroupName = group.ToDescription();
			IsExpanded = true;
		}

		public string GroupName { get; private set; }
	}
}
using RubezhAPI;
using Infrastructure.Common.Windows.Services.Layout;
using Infrastructure.Common.Windows.TreeList;

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
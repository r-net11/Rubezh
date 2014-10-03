using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveDirectionViewModel : CheckBoxItemViewModel
	{
		public ArchiveDirectionViewModel(GKDirection direction)
		{
			Direction = direction;
			Name = direction.PresentationName;
		}

		public GKDirection Direction { get; private set; }
		public string Name { get; private set; }
	}
}
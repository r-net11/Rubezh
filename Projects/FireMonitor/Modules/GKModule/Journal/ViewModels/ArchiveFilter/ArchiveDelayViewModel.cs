using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchiveDelayViewModel : CheckBoxItemViewModel
	{
		public ArchiveDelayViewModel(XDelay delay)
		{
			Delay = delay;
			Name = delay.PresentationName;
		}

		public XDelay Delay { get; private set; }
		public string Name { get; private set; }
	}
}
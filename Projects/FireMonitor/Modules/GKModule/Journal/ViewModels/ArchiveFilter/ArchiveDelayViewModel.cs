using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

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
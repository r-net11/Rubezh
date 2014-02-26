using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchivePimViewModel : CheckBoxItemViewModel
	{
		public ArchivePimViewModel(XPim pim)
		{
			Pim = pim;
			Name = pim.PresentationName;
		}

		public XPim Pim { get; private set; }
		public string Name { get; private set; }
	}
}
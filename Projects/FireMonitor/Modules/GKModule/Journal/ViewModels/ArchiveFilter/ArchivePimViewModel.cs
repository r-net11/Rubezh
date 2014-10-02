using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchivePimViewModel : CheckBoxItemViewModel
	{
		public ArchivePimViewModel(GKPim pim)
		{
			Pim = pim;
			Name = pim.PresentationName;
		}

		public GKPim Pim { get; private set; }
		public string Name { get; private set; }
	}
}
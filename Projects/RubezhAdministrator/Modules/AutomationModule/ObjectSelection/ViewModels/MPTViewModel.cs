using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public GKMPT MPT { get; private set; }

		public MPTViewModel(GKMPT mpt)
		{
			MPT = mpt;
		}
	}
}

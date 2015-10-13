using RubezhAPI.GK;
using Infrastructure.Client.Plans.Presenter;

namespace GKModule.ViewModels
{
	public class MPTTooltipViewModel : StateTooltipViewModel<GKMPT>
	{
		public GKMPT MPT
		{
			get { return Item; }
		}

		public MPTTooltipViewModel(GKMPT mpt)
			: base(mpt)
		{
		}
	}
}
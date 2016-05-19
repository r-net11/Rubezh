using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

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
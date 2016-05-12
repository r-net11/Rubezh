using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	/// <summary>
	/// View Model for a Delay's Tooltip.
	/// </summary>
	public class DelayTooltipViewModel : StateTooltipViewModel<GKDelay>
	{
		/// <summary>
		/// Retrieves the Delay this Tooltip is for.
		/// </summary>
		public GKDelay Delay
		{
			get { return base.Item; }
		}

		/// <summary>
		/// Initializes a new Instance of current Class.
		/// </summary>
		/// <param name="delay">Delay this Tooltip is for.</param>
		public DelayTooltipViewModel(GKDelay delay)
			: base(delay)
		{
		}
	}
}

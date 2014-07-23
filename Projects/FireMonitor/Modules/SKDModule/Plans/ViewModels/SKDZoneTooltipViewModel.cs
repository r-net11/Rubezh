using FiresecAPI.SKD;
using Infrastructure.Client.Plans.Presenter;

namespace SKDModule.ViewModels
{
	public class SKDZoneTooltipViewModel : StateTooltipViewModel<SKDZone>
	{
		public SKDZone Zone
		{
			get { return Item; }
		}

		public SKDZoneTooltipViewModel(SKDZone zone)
			: base(zone)
		{
		}

		public override void OnStateChanged()
		{
			base.OnStateChanged();
			OnPropertyChanged(() => Zone);
		}
	}
}
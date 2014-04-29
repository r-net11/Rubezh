using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using SKDManager = FiresecAPI.SKDManager;

namespace SKDModule.ViewModels
{
	public class ScheduleZoneViewModel : BaseObjectViewModel<ScheduleZone>
	{
		public ScheduleZoneViewModel(ScheduleZone scheduleZone)
			: base(scheduleZone)
		{
		}

		public string Name
		{
			get
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				return zone == null ? null : zone.Name;
			}
		}

		public bool IsControl
		{
			get { return Model.IsControl; }
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => IsControl);
		}
	}
}
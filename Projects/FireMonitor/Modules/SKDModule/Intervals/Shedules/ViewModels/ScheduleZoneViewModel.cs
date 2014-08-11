using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

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
				return zone == null ? "<Нет в конфигурации>" : zone.Name;
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Name);
		}
	}
}
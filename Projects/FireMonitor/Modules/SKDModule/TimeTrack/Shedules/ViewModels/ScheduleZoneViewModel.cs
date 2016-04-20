using System.Linq;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient;
using RubezhAPI;

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
				var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (gkZone != null)
					return gkZone.Name;
				return "<Нет в конфигурации>";
			}
		}

		public int No
		{
			get
			{
				var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				if (gkZone != null)
					return gkZone.No;
				return default(int);
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Name);
		}
	}
}
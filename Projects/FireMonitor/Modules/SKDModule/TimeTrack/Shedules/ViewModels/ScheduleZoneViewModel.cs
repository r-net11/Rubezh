using System.Linq;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

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
				var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				return strazhZone != null ? strazhZone.Name : CommonResources.NotInConfig;
			}
		}

		public int No
		{
			get
			{
				var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == Model.ZoneUID);
				return strazhZone != null ? strazhZone.No : default(int);
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Name);
		}
	}
}
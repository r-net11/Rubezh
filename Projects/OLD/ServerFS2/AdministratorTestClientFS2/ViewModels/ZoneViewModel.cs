using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public Zone Zone { get; private set; }

		public ZoneViewModel(Zone zone)
		{
			Zone = zone;
		}

		public string DetectorCount
		{
			get
			{
				if (Zone.ZoneType == ZoneType.Fire)
					return Zone.DetectorCount.ToString();
				return null;
			}
		}

		public void Update(Zone zone)
		{
			Zone = zone;
			OnPropertyChanged("Zone");
			OnPropertyChanged("DetectorCount");
		}
	}
}
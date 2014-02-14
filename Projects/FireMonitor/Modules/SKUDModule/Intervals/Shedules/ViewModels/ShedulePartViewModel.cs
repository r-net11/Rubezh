using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ShedulePartViewModel : BaseViewModel
	{
		SheduleViewModel SheduleViewModel;
		public EmployeeShedulePart ShedulePart { get; set; }

		public ShedulePartViewModel(SheduleViewModel sheduleViewModel, EmployeeShedulePart shedulePart)
		{
			SheduleViewModel = sheduleViewModel;
			ShedulePart = shedulePart;
		}

		public string Name
		{
			get
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == ShedulePart.ZoneUID);
				if (zone != null)
				{
					return zone.Name;
				}
				return null;
			}
		}

		public bool IsControl
		{
			get { return ShedulePart.IsControl; }
		}

		public void Update()
		{
			OnPropertyChanged("ShedulePart");
			OnPropertyChanged("Name");
			OnPropertyChanged("IsControl");
		}
	}
}
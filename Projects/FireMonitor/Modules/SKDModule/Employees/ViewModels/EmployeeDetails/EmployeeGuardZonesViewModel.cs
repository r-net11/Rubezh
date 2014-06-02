using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeGuardZonesViewModel : BaseViewModel
	{
		public Employee Employee { get; private set; }

		public EmployeeGuardZonesViewModel(Employee employee)
		{
			Employee = employee;
			GuardZones = new ObservableCollection<EmployeeGuardZoneViewModel>();
			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
			{
				var guardZoneViewModel = new EmployeeGuardZoneViewModel(employee, guardZone);
				GuardZones.Add(guardZoneViewModel);
			}
			SelectedGuardZone = GuardZones.FirstOrDefault();
		}

		ObservableCollection<EmployeeGuardZoneViewModel> _guardZones;
		public ObservableCollection<EmployeeGuardZoneViewModel> GuardZones
		{
			get { return _guardZones; }
			private set
			{
				_guardZones = value;
				OnPropertyChanged("GuardZones");
			}
		}

		EmployeeGuardZoneViewModel _selectedGuardZone;
		public EmployeeGuardZoneViewModel SelectedGuardZone
		{
			get { return _selectedGuardZone; }
			set
			{
				_selectedGuardZone = value;
				OnPropertyChanged("SelectedGuardZone");
			}
		}
	}
}
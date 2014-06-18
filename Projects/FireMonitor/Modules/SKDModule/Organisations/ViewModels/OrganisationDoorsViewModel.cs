using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationDoorsViewModel(Organisation organisation)
		{
			Organisation = organisation;

			var doors = SKDManager.SKDConfiguration.Doors;

			Doors = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in doors)
			{
				Doors.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}

		ObservableCollection<OrganisationDoorViewModel> _doors;
		public ObservableCollection<OrganisationDoorViewModel> Doors
		{
			get { return _doors; }
			private set
			{
				_doors = value;
				OnPropertyChanged(()=>Doors);
			}
		}

		OrganisationDoorViewModel _selectedDoor;
		public OrganisationDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(()=>SelectedDoor);
			}
		}
	}
}
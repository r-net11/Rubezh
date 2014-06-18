using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }
		public Door Door { get; private set; }

		public OrganisationDoorViewModel(Organisation organisation, Door door)
		{
			Organisation = organisation;
			Door = door;
			_isChecked = Organisation != null && Organisation.DoorUIDs.Contains(door.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");

				if (value)
				{
					if (!Organisation.DoorUIDs.Contains(Door.UID))
						Organisation.DoorUIDs.Add(Door.UID);
				}
				else
				{
					if (Organisation.DoorUIDs.Contains(Door.UID))
						Organisation.DoorUIDs.Remove(Door.UID);
				}

				var saveResult = OrganisationHelper.SaveDoors(Organisation);
			}
		}
	}
}
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public SKDDoor Door { get; private set; }

		public OrganisationDoorViewModel(Organisation organisation, SKDDoor door)
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
				if (value)
				{
					if (!Organisation.DoorUIDs.Contains(Door.UID))
						Organisation.DoorUIDs.Add(Door.UID);
				}
				else
				{
					if (HasLinkedCards())
					{
						MessageBoxService.Show("Операция запрещена\nСуществуют карты, привязанные к данной точке доступа");
						OnPropertyChanged(() => IsChecked);
						return;
					}	
					if (Organisation.DoorUIDs.Contains(Door.UID))
					{
						Organisation.DoorUIDs.Remove(Door.UID);
					}
				}
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				var saveResult = OrganisationHelper.SaveDoors(Organisation);
			}
		}

		bool HasLinkedCards()
		{
			var cards = CardHelper.Get(new CardFilter());
			if (cards == null)
				return false;
			return cards.Any(x => x.OrganisationUID == Organisation.UID && x.CardDoors.Any(y => y.DoorUID == Door.UID));
		}
	}
}
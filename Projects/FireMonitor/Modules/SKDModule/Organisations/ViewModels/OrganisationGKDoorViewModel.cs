using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class OrganisationGKDoorViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public GKDoor Door { get; private set; }

		public OrganisationGKDoorViewModel(Organisation organisation, GKDoor door)
		{
			Organisation = organisation;
			Door = door;
			_isChecked = Organisation != null && Organisation.GKDoorUIDs.Contains(door.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (value)
				{
					if (!Organisation.GKDoorUIDs.Contains(Door.UID))
						Organisation.GKDoorUIDs.Add(Door.UID);
				}
				else
				{
					if (HasLinkedCards())
					{
						MessageBoxService.Show("Операция запрещена\nСуществуют карты, привязанные к данной точке доступа");
						OnPropertyChanged(() => IsChecked);
						return;
					}	
					if (Organisation.GKDoorUIDs.Contains(Door.UID))
					{
						Organisation.GKDoorUIDs.Remove(Door.UID);
					}
				}
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				var saveResult = OrganisationHelper.SaveGKDoors(Organisation);
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
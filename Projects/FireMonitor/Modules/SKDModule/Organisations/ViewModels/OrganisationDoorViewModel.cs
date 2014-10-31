using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		Organisation Organisation;
		Guid DoorUID;
		public string PresentationName { get; private set; }

		public OrganisationDoorViewModel(Organisation organisation, SKDDoor door)
		{
			Organisation = organisation;
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
			_isChecked = Organisation != null && Organisation.DoorUIDs.Contains(door.UID);
		}

		public OrganisationDoorViewModel(Organisation organisation, GKDoor door)
		{
			Organisation = organisation;
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
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
					if (!Organisation.DoorUIDs.Contains(DoorUID))
						Organisation.DoorUIDs.Add(DoorUID);
				}
				else
				{
					if (HasLinkedCards())
					{
						MessageBoxService.Show("Операция запрещена\nСуществуют карты, привязанные к данной точке доступа");
						OnPropertyChanged(() => IsChecked);
						return;
					}
					if (HasLinkedAccessTemplates())
					{
						MessageBoxService.Show("Операция запрещена\nСуществуют шаблоны доступа, привязанные к данной точке доступа");
						OnPropertyChanged(() => IsChecked);
						return;
					}
					if (Organisation.DoorUIDs.Contains(DoorUID))
					{
						Organisation.DoorUIDs.Remove(DoorUID);
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
			return cards.Any(x => x.OrganisationUID == Organisation.UID && x.CardDoors.Any(y => y.DoorUID == DoorUID));
		}

		bool HasLinkedAccessTemplates()
		{
			var accessTemplates = AccessTemplateHelper.Get(new AccessTemplateFilter());
			if (accessTemplates == null)
				return false;
			return accessTemplates.Any(x => !x.IsDeleted && x.OrganisationUID == Organisation.UID && x.CardDoors.Any(y => y.DoorUID == DoorUID));
		}
	}
}
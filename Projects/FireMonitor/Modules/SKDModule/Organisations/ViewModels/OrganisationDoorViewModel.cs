using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System;
using System.Collections.Generic;

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
					var linkedCards = new List<SKDCard>();
					var cardsHelper = CardHelper.Get(new CardFilter());
					if (cardsHelper != null)
					{
						linkedCards = cardsHelper.Where(x => x.OrganisationUID == Organisation.UID && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
					}

					var linkedAccessTemplates = new List<AccessTemplate>();
					var accessTemplatesHelper = AccessTemplateHelper.Get(new AccessTemplateFilter());
					if (accessTemplatesHelper != null)
					{
						linkedAccessTemplates = accessTemplatesHelper.Where(x => !x.IsDeleted && x.OrganisationUID == Organisation.UID && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
					}

					if (linkedCards.Count > 0 || linkedAccessTemplates.Count > 0)
					{
						if (MessageBoxService.ShowQuestion("Существуют карты или шаблоны доступа, привязанные к данной точке доступа\nВы уверены, что хотите снять права с точки доступа?"))
						{
							if (Organisation.DoorUIDs.Contains(DoorUID))
							{
								Organisation.DoorUIDs.Remove(DoorUID);
							}

							foreach (var card in linkedCards)
							{
								card.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
								CardHelper.Edit(card);
							}

							foreach (var accessTemplate in linkedAccessTemplates)
							{
								accessTemplate.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
								AccessTemplateHelper.Save(accessTemplate, false);
							}
						}
					}

					OnPropertyChanged(() => IsChecked);
				}
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				var saveResult = OrganisationHelper.SaveDoors(Organisation);
			}
		}
	}
}
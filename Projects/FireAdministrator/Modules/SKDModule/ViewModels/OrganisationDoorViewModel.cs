using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		private readonly Organisation _organisation;
		private readonly Guid _doorUid;

		public string Name { get; private set; }
		public int No { get; private set; }

		public OrganisationDoorViewModel(Organisation organisation, SKDDoor door)
		{
			_organisation = organisation;
			_doorUid = door.UID;
			Name = door.Name;
			No = door.No;
			_isChecked = _organisation != null && _organisation.DoorUIDs.Contains(door.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (value)
				{
					if (!_organisation.DoorUIDs.Contains(_doorUid))
						_organisation.DoorUIDs.Add(_doorUid);
					var saveResult = OrganisationHelper.SaveDoors(_organisation);
					if (saveResult)
					{
						_isChecked = value;
						OnPropertyChanged(() => IsChecked);
					}
				}
				else
				{
					var linkedCards = new List<SKDCard>();
					var cardsHelper = CardHelper.Get(new CardFilter());
					if (cardsHelper != null)
					{
						linkedCards = cardsHelper.Where(x => x.OrganisationUID == _organisation.UID && x.CardDoors.Any(y => y.DoorUID == _doorUid)).ToList();
					}

					var linkedAccessTemplates = new List<AccessTemplate>();
					var accessTemplatesHelper = AccessTemplateHelper.Get(new AccessTemplateFilter());
					if (accessTemplatesHelper != null)
					{
						linkedAccessTemplates = accessTemplatesHelper.Where(x => !x.IsDeleted && x.OrganisationUID == _organisation.UID && x.CardDoors.Any(y => y.DoorUID == _doorUid)).ToList();
					}

					var schedules = ScheduleHelper.Get(new ScheduleFilter());
					var hasLinkedSchedules = schedules.Any(x => x.OrganisationUID == _organisation.UID && x.Zones.Any(y => y.DoorUID == _doorUid));

					var canRemove = true;
					if (linkedCards.Count > 0 || linkedAccessTemplates.Count > 0 || hasLinkedSchedules)
					{
						if (MessageBoxService.ShowQuestion("Существуют карты, шаблоны доступа или графики, привязанные к данной точке доступа\nВы уверены, что хотите снять права с точки доступа?"))
						{
							foreach (var card in linkedCards)
							{
								card.CardDoors.RemoveAll(x => x.DoorUID == _doorUid);
								CardHelper.Edit(card, _organisation.Name);
							}

							foreach (var accessTemplate in linkedAccessTemplates)
							{
								accessTemplate.CardDoors.RemoveAll(x => x.DoorUID == _doorUid);
								AccessTemplateHelper.Save(accessTemplate, false);
							}
						}
						else
						{
							canRemove = false;
						}
					}

					if (canRemove)
					{
						if (_organisation.DoorUIDs.Contains(_doorUid))
						{
							_organisation.DoorUIDs.Remove(_doorUid);
						}
						var saveResult = OrganisationHelper.SaveDoors(_organisation);
						if (saveResult)
						{
							_isChecked = value;
							OnPropertyChanged(() => IsChecked);
						}
					}
				}
			}
		}
	}
}
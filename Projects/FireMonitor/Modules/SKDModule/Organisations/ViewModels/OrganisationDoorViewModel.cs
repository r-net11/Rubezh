using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		Organisation Organisation;
		Guid DoorUID;
		public string Name { get; private set; }
		public int No { get; private set; }

		public OrganisationDoorViewModel(Organisation organisation, SKDDoor door)
		{
			Organisation = organisation;
			DoorUID = door.UID;
			Name = door.Name;
			No = door.No;
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
					var saveResult = OrganisationHelper.SaveDoors(Organisation);
					if (saveResult)
					{
						_isChecked = value;
						OnPropertyChanged(() => IsChecked);
						ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Publish(Organisation.UID);
					}
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

					var schedules = ScheduleHelper.Get(new ScheduleFilter());
					var hasLinkedSchedules = schedules.Any(x => x.OrganisationUID == Organisation.UID && x.Zones.Any(y => y.DoorUID == DoorUID));

					bool canRemove = true;
					if (linkedCards.Count > 0 || linkedAccessTemplates.Count > 0 || hasLinkedSchedules)
					{
						if (MessageBoxService.ShowQuestion("Существуют карты, шаблоны доступа или графики, привязанные к данной точке доступа\nВы уверены, что хотите снять права с точки доступа?"))
						{
							foreach (var card in linkedCards)
							{
								card.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
								CardHelper.Edit(card, Organisation.Name);
							}

							foreach (var accessTemplate in linkedAccessTemplates)
							{
								accessTemplate.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
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
						if (Organisation.DoorUIDs.Contains(DoorUID))
						{
							Organisation.DoorUIDs.Remove(DoorUID);
						}
						var saveResult = OrganisationHelper.SaveDoors(Organisation);
						if (saveResult)
						{
							_isChecked = value;
							OnPropertyChanged(() => IsChecked);
							ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Publish(Organisation.UID);
						}
					}
				}
			}
		}
	}
}
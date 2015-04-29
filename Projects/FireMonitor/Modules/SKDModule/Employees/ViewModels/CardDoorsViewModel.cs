using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class CardDoorsViewModel : BaseViewModel
	{
		public List<CardDoor> CardDoors { get; private set; }
		ICardDoorsParent _parent;

		public CardDoorsViewModel(List<CardDoor> cardDoors, ICardDoorsParent parent)
		{
			Update(cardDoors);
			_parent = parent;
			ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Unsubscribe(OnOrganisationDoorsChanged);
			ServiceFactory.Events.GetEvent<UpdateOrganisationDoorsEvent>().Subscribe(OnOrganisationDoorsChanged);
		}

		public void Update(List<CardDoor> cardDoors)
		{
			CardDoors = cardDoors;
			InitializeDoors();
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<ReadOnlyAccessDoorViewModel>();
			foreach (var cardDoor in CardDoors)
			{
				var skdDoor = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (skdDoor != null)
				{
					var doorViewModel = new ReadOnlyAccessDoorViewModel(skdDoor, cardDoor);
					Doors.Add(doorViewModel);
				}
				else
				{
					var gkDoor = GKManager.DeviceConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
					if (gkDoor != null)
					{
						var doorViewModel = new ReadOnlyAccessDoorViewModel(gkDoor, cardDoor);
						Doors.Add(doorViewModel);
					}
				}
			}
		}

		ObservableCollection<ReadOnlyAccessDoorViewModel> _doors;
		public ObservableCollection<ReadOnlyAccessDoorViewModel> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

		ReadOnlyAccessDoorViewModel _selectedDoor;
		public ReadOnlyAccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
			}
		}

		void OnOrganisationDoorsChanged(Guid organisationUID)
		{
			var doorUIDs = OrganisationHelper.GetSingle(organisationUID).DoorUIDs;
			var doorsToRemove = Doors.Where(x => !doorUIDs.Any(y => y == x.CardDoor.DoorUID)).ToList();
			doorsToRemove.ForEach(x => Doors.Remove(x));
			_parent.UpdateCardDoors(doorsToRemove.Select(x => x.CardDoor.DoorUID));
		}
	}

	public interface ICardDoorsParent
	{
		void UpdateCardDoors(IEnumerable<Guid> doorUIDs);
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class CardDoorsViewModel : BaseViewModel
	{
		public List<CardDoor> CardDoors { get; private set; }
		IDoorsParent _parent;

		public CardDoorsViewModel(List<CardDoor> cardDoors, IDoorsParent parent)
		{
			Update(cardDoors);
			_parent = parent;
		}

		public void Update(List<CardDoor> cardDoors)
		{
			CardDoors = cardDoors;
			InitializeDoors();
		}

		void InitializeDoors()
		{
			var schedules = GKScheduleHelper.GetSchedules();
			Doors = new ObservableCollection<ReadOnlyAccessDoorViewModel>();
			var gkDoors = from cardDoor in CardDoors
				join gkDoor in  GKManager.DeviceConfiguration.Doors on cardDoor.DoorUID equals gkDoor.UID
				select new { CardDoor = cardDoor, GKDoor = gkDoor};
			foreach (var doorViewModel in gkDoors.Select(x => new ReadOnlyAccessDoorViewModel(x.GKDoor, x.CardDoor, schedules)).OrderBy(x=>x.PresentationName))
				Doors.Add(doorViewModel);
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

		public void UpdateDoors(IEnumerable<Guid> doorUIDs)
		{
			var doorsToRemove = Doors.Where(x => !doorUIDs.Any(y => y == x.CardDoor.DoorUID)).ToList();
			doorsToRemove.ForEach(x => Doors.Remove(x));
		}
	}
}
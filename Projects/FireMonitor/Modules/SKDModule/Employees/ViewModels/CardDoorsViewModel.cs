using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

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
			if (CardDoors == null) return;

			Doors = new SortableObservableCollection<ReadOnlyAccessDoorViewModel>();
			foreach (var cardDoor in CardDoors)
			{
				var skdDoor = SKDManager.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (skdDoor != null)
				{
					var doorViewModel = new ReadOnlyAccessDoorViewModel(skdDoor, cardDoor);
					Doors.Add(doorViewModel);
				}
			}
		}

		SortableObservableCollection<ReadOnlyAccessDoorViewModel> _doors;
		public SortableObservableCollection<ReadOnlyAccessDoorViewModel> Doors
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
			var doorsToRemove = Doors.Where(x => doorUIDs.All(y => y != x.CardDoor.DoorUID)).ToList();
			doorsToRemove.ForEach(x => Doors.Remove(x));
		}
	}
}
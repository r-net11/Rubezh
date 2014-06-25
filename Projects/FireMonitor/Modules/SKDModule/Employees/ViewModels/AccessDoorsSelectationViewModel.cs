﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessDoorsSelectationViewModel : BaseViewModel
	{
		Organisation Organisation;
		public List<CardDoor> CardDoors { get; private set; }
		Guid? ParentUID;

		public AccessDoorsSelectationViewModel(Organisation organisation, List<CardDoor> cardDoors, Guid? parentUID)
		{
			Organisation = organisation;
			CardDoors = cardDoors;

			InitializeDoors();
			ParentUID = parentUID;

			foreach (var zone in Doors)
			{
				zone.ExpandToThis();
			}
		}

		void InitializeDoors()
		{
			Doors = new ObservableCollection<AccessDoorViewModel>();
			var organisationDoors = SKDManager.SKDConfiguration.Doors.Where(x => Organisation.DoorUIDs.Any(y => y == x.UID));
			foreach (var door in organisationDoors)
			{
				var accessDoorViewModel = new AccessDoorViewModel(door, CardDoors, x => { SelectedDoor = x; });
				Doors.Add(accessDoorViewModel);
			}
		}

		public ObservableCollection<AccessDoorViewModel> Doors { get; private set; }

		AccessDoorViewModel _selectedDoor;
		public AccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedDoor");
			}
		}

		public List<CardDoor> GetCardDoors()
		{
			CardDoors = new List<CardDoor>();
			foreach (var door in Doors)
			{
				if (door.IsChecked)
				{
					var cardDoor = new CardDoor()
					{
						DoorUID = door.Door.UID,
						IsAntiPassback = door.IsAntiPassback,
						IsComission = door.IsComission,
						EnterIntervalType = door.SelectedEnterTimeCreteria.IntervalType,
						EnterIntervalUID = door.SelectedEnterTimeType.UID,
						ExitIntervalType = door.SelectedExitTimeCreteria.IntervalType,
						ExitIntervalUID = door.SelectedExitTimeType.UID,
						ParentUID = ParentUID
					};
					CardDoors.Add(cardDoor);
				}
			}
			return CardDoors;
		}
	}
}
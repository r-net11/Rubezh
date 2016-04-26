using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Microsoft.Practices.Prism;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class AccessDoorsSelectationViewModel : BaseViewModel
	{
		#region Constructors

		public AccessDoorsSelectationViewModel(Organisation organisation, List<CardDoor> cardDoors)
		{
			CardDoors = cardDoors ?? new List<CardDoor>();

			Doors = new ObservableCollection<AccessDoorViewModel>();

			var skdDoors = GetDoors(SKDManager.SKDConfiguration.Doors, organisation);
			var accessDoorViewModels =
				skdDoors
				.Select(skdDoor => new AccessDoorViewModel(skdDoor, CardDoors, x =>
				{
					SelectedDoor = x;
					ServiceFactory.Events.GetEvent<AccessTemplateDoorsSelectedEvent>().Publish(GetSelectedDoorsUids());
				}))
				.ToList();

			Doors.AddRange(accessDoorViewModels);

			SelectedDoor = Doors.FirstOrDefault(x => x.IsChecked);
		}

		public List<Guid> GetSelectedDoorsUids()
		{
			var result = Doors.Where(x => x.IsChecked).Select(y => y.DoorUID).ToList();
			return result;
		}

		public AccessDoorsSelectationViewModel()
		{
		}

		#endregion

		#region Properties
		public List<CardDoor> CardDoors { get; private set; }
		public ObservableCollection<AccessDoorViewModel> Doors { get; private set; }

		AccessDoorViewModel _selectedDoor;
		public AccessDoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
				HasSelectedDoor = value != null;
			}
		}

		bool _hasSelectedDoor;
		public bool HasSelectedDoor
		{
			get { return _hasSelectedDoor; }
			set
			{
				_hasSelectedDoor = value;
				OnPropertyChanged(() => HasSelectedDoor);
			}
		}
		#endregion

		#region Methods

		private List<SKDDoor> GetDoors(List<SKDDoor> skdDoors, Organisation organisation)
		{
			return skdDoors.Where(door => organisation.DoorUIDs.Any(y => y == door.UID)).ToList();
		}

		public List<CardDoor> GetCardDoors()
		{
			CardDoors = new List<CardDoor>();

			foreach (var door in Doors)
			{
				if (!door.IsChecked) continue;

				CardDoors.Add(new CardDoor
					{
						DoorUID = door.DoorUID,
						EnterScheduleNo = door.SelectedEnterSchedule != null ? door.SelectedEnterSchedule.ScheduleNo : default(int),
					}
				);
			}

			return CardDoors;
		}

		#endregion
	}
}
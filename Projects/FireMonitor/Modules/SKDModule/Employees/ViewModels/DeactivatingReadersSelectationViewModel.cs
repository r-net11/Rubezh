using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DeactivatingReadersSelectationViewModel : BaseViewModel
	{
		public ObservableCollection<DeactivatingReaderSelectationViewModel> Readers { get; private set; }

		public DeactivatingReadersSelectationViewModel(IEnumerable<Guid> doorUIDs)
		{
			Readers = new ObservableCollection<DeactivatingReaderSelectationViewModel>();
			BuildTree(doorUIDs);
			ServiceFactory.Events.GetEvent<AccessTemplateDoorsSelectedEvent>().Subscribe(BuildTree);
		}

		private void BuildTree(IEnumerable<Guid> doorUIDs)
		{
			Readers.Clear();
			
			var doors = SKDManager.Doors.Where(door => doorUIDs.Any(doorUID => doorUID == door.UID)).ToList();
			foreach (var door in doors)
			{
				Readers.Add(BuildDoor(door));
			}
		}

		private DeactivatingReaderSelectationViewModel BuildDoor(SKDDoor door)
		{
			// Точка доступа
			var readerViewModel = new DeactivatingReaderSelectationViewModel(false)
			{
				UID = door.UID,
				Name = door.Name,
				IsExpanded = true
			};

			// Связанные с точкой доступа считыватели
			foreach (var linkedDevice in new [] { door.InDevice, door.OutDevice})
			{
				if (linkedDevice != null && linkedDevice.DriverType == SKDDriverType.Reader)
				{
					var inReaderViewModel = new DeactivatingReaderSelectationViewModel(true)
					{
						UID = linkedDevice.UID,
						Name = linkedDevice.Name,
						ZoneName = linkedDevice.Zone.Name
					};
					readerViewModel.AddChild(inReaderViewModel);
				}
			}

			return readerViewModel;
		}
	}
}
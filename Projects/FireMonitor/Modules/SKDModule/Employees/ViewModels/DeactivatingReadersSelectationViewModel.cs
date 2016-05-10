using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DeactivatingReadersSelectationViewModel : BaseViewModel
	{
		public ObservableCollection<DeactivatingReaderSelectationViewModel> Readers { get; private set; }

		public DeactivatingReadersSelectationViewModel(IEnumerable<Guid> doorUIDs, List<AccessTemplateDeactivatingReader> deactivatingReaders)
		{
			Readers = new ObservableCollection<DeactivatingReaderSelectationViewModel>();
			BuildTree(doorUIDs, deactivatingReaders);
			ServiceFactory.Events.GetEvent<AccessTemplateDoorsSelectedEvent>().Subscribe(doors => BuildTree(doors));
		}

		private void BuildTree(IEnumerable<Guid> doorUIDs, List<AccessTemplateDeactivatingReader> deactivatingReaders = null)
		{
			var doors = SKDManager.Doors.Where(door => doorUIDs.Any(doorUID => doorUID == door.UID)).ToList();
			var checkedReaderGuids = GetDeactivatingReaderUIDs(deactivatingReaders);
			
			Readers.Clear();
			foreach (var door in doors)
			{
				Readers.Add(BuildDoor(door, checkedReaderGuids));
			}
		}

		private List<Guid> GetDeactivatingReaderUIDs(List<AccessTemplateDeactivatingReader> deactivatingReaders)
		{
			var result = new List<Guid>();

			if (deactivatingReaders == null)
			{
				foreach (var door in Readers)
				{
					foreach (var reader in door.Children)
					{
						if (reader.IsChecked)
							result.Add(reader.UID);
					}
				}
			}
			else
			{
				result.AddRange(deactivatingReaders.Select(x => x.DeactivatingReaderUID));
			}

			return result;
		}

		private DeactivatingReaderSelectationViewModel BuildDoor(SKDDoor door, List<Guid> checkedReaderGuids)
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
						ZoneName = linkedDevice.Zone.Name,
						IsChecked = checkedReaderGuids.Any(x => x == linkedDevice.UID)
					};
					readerViewModel.AddChild(inReaderViewModel);
				}
			}

			return readerViewModel;
		}

		public List<AccessTemplateDeactivatingReader> GetReaders()
		{
			var result = new List<AccessTemplateDeactivatingReader>();

			foreach (var door in Readers)
			{
				foreach (var reader in door.Children)
				{
					if (reader.IsChecked)
						result.Add(new AccessTemplateDeactivatingReader()
						{
							DeactivatingReaderUID = reader.UID
						});
				}
			}
			
			return result;
		}
	}
}
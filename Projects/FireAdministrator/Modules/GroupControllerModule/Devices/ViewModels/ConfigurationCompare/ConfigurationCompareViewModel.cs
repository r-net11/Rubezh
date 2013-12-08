using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ConfigurationCompareViewModel : DialogViewModel
	{
		XDevice LocalDevice { get; set; }
		XDevice RemoteDevice { get; set; }
		XDeviceConfiguration LocalConfiguration { get; set; }
		XDeviceConfiguration RemoteConfiguration { get; set; }
		public ObjectsListViewModel LocalObjectsViewModel { get; set; }
		public ObjectsListViewModel RemoteObjectsViewModel { get; set; }
		public static bool ConfigFromFile { get; private set; }

		public ConfigurationCompareViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration, XDevice device, bool configFromFile)
		{
			Title = "Сравнение конфигураций " + device.PresentationName;
			ConfigFromFile = configFromFile;
			ChangeCommand = new RelayCommand(OnChange);
			NextDifferenceCommand = new RelayCommand(OnNextDifference, CanNextDifference);
			PreviousDifferenceCommand = new RelayCommand(OnPreviousDifference, CanPreviousDifference);

			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;
			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType && x.Address == device.Address);
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType && x.Address == device.Address);

			LocalObjectsViewModel = new ObjectsListViewModel(LocalDevice, localConfiguration);
			RemoteObjectsViewModel = new ObjectsListViewModel(RemoteDevice, remoteConfiguration);
			CompareObjectLists();
			InitializeMismatchedIndexes();
		}
		
		List<int> mismatchedIndexes;
		void InitializeMismatchedIndexes()
		{
			mismatchedIndexes = new List<int>();
			for (int i = 0; i < LocalObjectsViewModel.Objects.Count; i++)
			{
				var item = LocalObjectsViewModel.Objects[i];
				if ((item.IsAbsent || item.HasNonStructureDifferences || RemoteObjectsViewModel.Objects[i].IsAbsent || RemoteObjectsViewModel.Objects[i].HasNonStructureDifferences) && !mismatchedIndexes.Contains(i))
					mismatchedIndexes.Add(i);
			}
		}

		int SelectedIndex
		{
			get { return LocalObjectsViewModel.Objects.IndexOf(LocalObjectsViewModel.SelectedObject); }
		}

		public RelayCommand NextDifferenceCommand { get; private set; }
		void OnNextDifference()
		{
			var nextIndex = mismatchedIndexes.FirstOrDefault(x => x > SelectedIndex);
			LocalObjectsViewModel.SelectedObject = LocalObjectsViewModel.Objects[nextIndex];
		}
		bool CanNextDifference()
		{
			return mismatchedIndexes.Any(x => x > SelectedIndex);
		}

		public RelayCommand PreviousDifferenceCommand { get; private set; }
		void OnPreviousDifference()
		{
			var previousIndex = mismatchedIndexes.LastOrDefault(x => x < SelectedIndex);
			LocalObjectsViewModel.SelectedObject = LocalObjectsViewModel.Objects[previousIndex];
		}
		bool CanPreviousDifference()
		{
			return mismatchedIndexes.Any(x => x < SelectedIndex);
		}

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			RemoteDevice.UID = LocalDevice.UID;
			var rootDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.UID == LocalDevice.Parent.UID);
			rootDevice.Children.Remove(LocalDevice);
			rootDevice.Children.Add(RemoteDevice);
			if (LocalDevice.DriverType == XDriverType.GK)
			{
				LocalConfiguration.Zones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Zones.AddRange(RemoteConfiguration.Zones);
				LocalConfiguration.Directions.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Directions.AddRange(RemoteConfiguration.Directions);
			}
			ServiceFactory.SaveService.GKChanged = true;
			XManager.UpdateConfiguration();
			Close(true);
		}

		public void CompareObjectLists()
		{
			var unionObjects = CreateUnionObjectList(LocalObjectsViewModel.Objects, RemoteObjectsViewModel.Objects);
			var unionObjects1 = CreateOneComparedObjectList(LocalObjectsViewModel.Objects, RemoteObjectsViewModel.Objects, unionObjects, true);
			var unionObjects2 = CreateOneComparedObjectList(RemoteObjectsViewModel.Objects, LocalObjectsViewModel.Objects, unionObjects, false);
			LocalObjectsViewModel.Objects = unionObjects1;
			RemoteObjectsViewModel.Objects = unionObjects2;
		}
		List<ObjectViewModel> CreateUnionObjectList(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2)
		{
			var unionObjects = objects1.Select(object1 => (ObjectViewModel)object1.Clone()).ToList();
			foreach (var object2 in objects2)
			{
				if (unionObjects.All(x => x.Compare(x, object2) != 0))
					unionObjects.Add(object2);
			}
			unionObjects.Sort();
			return unionObjects;
		}
		List<ObjectViewModel> CreateOneComparedObjectList(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2, List<ObjectViewModel> unionObjects, bool IsLocalConfig)
		{
			var unionObjects1 = new List<ObjectViewModel>();
			foreach (var unionObject in unionObjects)
			{
				var newObject = (ObjectViewModel)unionObject.Clone();
				var sameObject1 = objects1.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
				if (sameObject1 == null)
				{
					newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в конфигурации" : "Отсутствует в приборе";
					newObject.IsAbsent = true;
				}
				else
				{
					var sameObject2 = objects2.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
					if (sameObject2 == null)
					{
						newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в приборе" : "Отсутствует в конфигурации";
						newObject.IsPresent = true;
					}
					else
					{
						if (sameObject1.PresentationZone != sameObject2.PresentationZone)
							newObject.DifferenceDiscription = sameObject1.Device.Driver.HasZone ? "Не совпадает зона" : "Не совпадает логика срабатывания";
						newObject.PresentationZone = sameObject1.PresentationZone;
						if (sameObject1.ObjectType == ObjectType.Zone)
						{
							newObject.DifferenceDiscription = GetZonesDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject2.Name;
						}
						if (sameObject1.ObjectType == ObjectType.Direction)
						{
							newObject.DifferenceDiscription = GetDirectionsDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject2.Name;
						}
					}

				}
				unionObjects1.Add(newObject);
			}
			return unionObjects1;
		}
		string GetZonesDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			string zonesDifferences = null;
			if (object1.Name != object2.Name)
				zonesDifferences = "Не совпадает название";
			if (object1.Zone.Fire1Count != object2.Zone.Fire1Count)
				zonesDifferences = "Не совпадает число датчиков для формирования Пожар1";
			if (object1.Zone.Fire2Count != object2.Zone.Fire2Count)
			{
				if (!String.IsNullOrEmpty(zonesDifferences))
					zonesDifferences += ", число датчиков для формирования Пожар2";
				else
					zonesDifferences = "Не совпадает число датчиков для формирования Пожар2";
			}
			return zonesDifferences;
		}
		string GetDirectionsDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			string directionsDifferences = null;
			if (object1.Name != object2.Name)
				directionsDifferences = "Не совпадает название";
			if (object1.Direction.Delay != object2.Direction.Delay)
				directionsDifferences = "Не совпадает параметр Задержка";
			if (object1.Direction.Hold != object2.Direction.Hold)
			{
				if (!String.IsNullOrEmpty(directionsDifferences))
					directionsDifferences += ", параметр Удержание";
				else
					directionsDifferences = "Не совпадает параметр Удержание";
			}
			if (object1.Direction.Regime != object2.Direction.Regime)
			{
				if (!String.IsNullOrEmpty(directionsDifferences))
					directionsDifferences += ", параметр Режим работы";
				else
					directionsDifferences = "Не совпадает параметр Режим работы";
			}
			return directionsDifferences;
		}
	}
}
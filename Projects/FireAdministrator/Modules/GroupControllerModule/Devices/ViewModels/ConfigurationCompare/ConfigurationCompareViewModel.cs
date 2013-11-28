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
				if ((item.IsAbsent || RemoteObjectsViewModel.Objects[i].IsAbsent) && !mismatchedIndexes.Contains(i))
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
			var objects1 = LocalObjectsViewModel.Objects;
			var objects2 = RemoteObjectsViewModel.Objects;

			var unionObjects = objects1.Select(object1 => (ObjectViewModel)object1.Clone()).ToList();
			foreach (var object2 in objects2)
			{
				if (unionObjects.All(x => x.Compare(x, object2) != 0))
					unionObjects.Add(object2);
			}
			unionObjects.Sort();

			var unionObjects1 = new List<ObjectViewModel>();
			foreach (var unionObject in unionObjects)
			{
				var newObject = (ObjectViewModel)unionObject.Clone();
				var sameObject1 = objects1.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
				if (sameObject1 == null)
				{
					newObject.HasDifferentsDiscription = "отсутствует в конфигурации";
					newObject.IsAbsent = true;
				}
				else
				{
					var sameObject2 = objects2.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
					if (sameObject2 == null)
					{
						newObject.HasDifferentsDiscription = "отсутствует в устройстве";
						newObject.IsPresent = true;
					}
					else
					{
						if (sameObject1.PresentationZone != sameObject2.PresentationZone)
						{
							if (sameObject1.Device.Driver.HasZone)
								newObject.HasDifferentsDiscription = "зоны различны";
							else
								newObject.HasDifferentsDiscription = "логика различна";
						}
						newObject.PresentationZone = sameObject1.PresentationZone;
						if (sameObject1.ObjectType == ObjectType.Zone)
						{
							if (sameObject1.Zone.Fire1Count != sameObject2.Zone.Fire1Count)
								newObject.HasDifferentsDiscription = "Не совпадает число датчиков для формирования Пожар1";
							if (sameObject1.Zone.Fire2Count != sameObject2.Zone.Fire2Count)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", число датчиков для формирования Пожар2";
								else
									newObject.HasDifferentsDiscription = "Не совпадает число датчиков для формирования Пожар2";
							}
						}
						if (sameObject1.ObjectType == ObjectType.Direction)
						{
							if (sameObject1.Direction.Delay != sameObject2.Direction.Delay)
								newObject.HasDifferentsDiscription = "Не совпадает параметр Задержка";
							if (sameObject1.Direction.Hold != sameObject2.Direction.Hold)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", параметр Удержание";
								else
									newObject.HasDifferentsDiscription = "Не совпадает параметр Удержание";
							}
							if (sameObject1.Direction.Regime != sameObject2.Direction.Regime)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", параметр Режим работы";
								else
									newObject.HasDifferentsDiscription = "Не совпадает параметр Режим работы";
							}
						}
					}

				}

				unionObjects1.Add(newObject);
			}

			var unionObjects2 = new List<ObjectViewModel>();
			foreach (var unionObject in unionObjects)
			{
				var newObject = (ObjectViewModel)unionObject.Clone();
				var sameObject2 = objects2.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
				if (sameObject2 == null)
				{
					newObject.HasDifferentsDiscription = "отсутствует в устройстве";
					newObject.IsAbsent = true;
				}
				else
				{
					var sameObject1 = objects1.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
					if (sameObject1 == null)
					{
						newObject.HasDifferentsDiscription = "отсутствует в конфигурации";
						newObject.IsPresent = true;
					}
					else
					{
						if (sameObject1.PresentationZone != sameObject2.PresentationZone)
						{
							if (sameObject1.Device.Driver.HasZone)
								newObject.HasDifferentsDiscription = "зоны различны";
							else
								newObject.HasDifferentsDiscription = "логика различна";
						}
						newObject.PresentationZone = sameObject2.PresentationZone;
						if (sameObject1.ObjectType == ObjectType.Zone)
						{
							if (sameObject1.Zone.Fire1Count != sameObject2.Zone.Fire1Count)
								newObject.HasDifferentsDiscription = "Не совпадает число датчиков для формирования Пожар1";
							if (sameObject1.Zone.Fire2Count != sameObject2.Zone.Fire2Count)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", число датчиков для формирования Пожар2";
								else
									newObject.HasDifferentsDiscription = "Не совпадает число датчиков для формирования Пожар2";
							}
						}
						if (sameObject1.ObjectType == ObjectType.Direction)
						{
							if (sameObject1.Direction.Delay != sameObject2.Direction.Delay)
								newObject.HasDifferentsDiscription = "Не совпадает параметр Задержка";
							if (sameObject1.Direction.Hold != sameObject2.Direction.Hold)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", параметр Удержание";
								else
									newObject.HasDifferentsDiscription = "Не совпадает параметр Удержание";
							}
							if (sameObject1.Direction.Regime != sameObject2.Direction.Regime)
							{
								if (!String.IsNullOrEmpty(newObject.HasDifferentsDiscription))
									newObject.HasDifferentsDiscription += ", параметр Режим работы";
								else
									newObject.HasDifferentsDiscription = "Не совпадает параметр Режим работы";
							}
						}
					}
				}
				unionObjects2.Add(newObject);
			}
			LocalObjectsViewModel.Objects = unionObjects1;
			RemoteObjectsViewModel.Objects = unionObjects2;
		}
	}
}
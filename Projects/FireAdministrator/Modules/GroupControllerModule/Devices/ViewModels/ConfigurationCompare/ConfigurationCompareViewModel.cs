using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ConfigurationCompareViewModel : DialogViewModel
	{
		GKDevice LocalDevice { get; set; }
		GKDevice RemoteDevice { get; set; }
		GKDeviceConfiguration LocalConfiguration { get; set; }
		GKDeviceConfiguration RemoteConfiguration { get; set; }
		public ObjectsListViewModel LocalObjectsViewModel { get; set; }
		public ObjectsListViewModel RemoteObjectsViewModel { get; set; }
		internal static bool ConfigFromFile { get; private set; }
		public string Error { get; private set; }

		public ConfigurationCompareViewModel(GKDeviceConfiguration localConfiguration, GKDeviceConfiguration remoteConfiguration, GKDevice device, bool configFromFile)
		{
			Title = "Сравнение конфигураций " + device.PresentationName;
			ConfigFromFile = configFromFile;
			ChangeCommand = new RelayCommand(OnChange, CanChange);
			NextDifferenceCommand = new RelayCommand(OnNextDifference, CanNextDifference);
			PreviousDifferenceCommand = new RelayCommand(OnPreviousDifference, CanPreviousDifference);

			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;
			RemoteConfiguration.Update();
			RemoteConfiguration.UpdateConfiguration();

			LocalDevice = localConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType && x.Address == device.Address);
			RemoteDevice = remoteConfiguration.Devices.FirstOrDefault(x => x.DriverType == device.DriverType && x.Address == device.Address);
			if (RemoteDevice == null)
			{
				Error = "ГК в удаленной конфигурации имеет невалидный IP адрес";
				return;
			}
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
			if (LocalDevice.DriverType == GKDriverType.GK)
			{
				LocalConfiguration.Doors.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Doors.AddRange(RemoteConfiguration.Doors);
				LocalConfiguration.SKDZones.Clear();
				LocalConfiguration.SKDZones.AddRange(RemoteConfiguration.SKDZones);
			}
			ServiceFactory.SaveService.GKChanged = true;
			GKManager.UpdateConfiguration();
			Close(true);
		}

		bool CanChange()
		{
			return ConfigFromFile;
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
					newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в локальной конфигурации" : "Отсутствует в конфигурации прибора";
					newObject.IsAbsent = true;
				}
				else
				{
					var sameObject2 = objects2.FirstOrDefault(x => x.Compare(x, unionObject) == 0);
					if (sameObject2 == null)
					{
						newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в конфигурации прибора" : "Отсутствует в локальной конфигурации";
						newObject.IsPresent = true;
					}
					else
					{
						if (sameObject1.ObjectType == ObjectType.Door)
						{
							newObject.DifferenceDiscription = GetDoorsDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
					}
				}
				unionObjects1.Add(newObject);
			}
			return unionObjects1;
		}

		string GetDoorsDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var differences = new StringBuilder();
			if (object1.Name != object2.Name)
				differences.Append("Не совпадает название");
			if (object1.Door.Delay != object2.Door.Delay)
			{
				if (differences.Length != 0)
					differences.Append(". ");
				differences.Append("Не совпадает задержка");
			}
			if (object1.Door.Hold != object2.Door.Hold)
			{
				if (differences.Length != 0)
					differences.Append(". ");
				differences.Append("Не совпадает удержание");
			}

			bool openLogicDiff = GKManager.GetPresentationLogic(object1.Door.OpenRegimeLogic) != GKManager.GetPresentationLogic(object2.Door.OpenRegimeLogic);
			if (openLogicDiff)
			{
				differences.Append("Не совпадают условия перевода в режим Всегда Включено");
			}

			return differences.ToString() == "" ? null : differences.ToString();
		}
	}
}
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
				LocalConfiguration.PumpStations.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.PumpStations.AddRange(RemoteConfiguration.PumpStations);
				LocalConfiguration.MPTs.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.MPTs.AddRange(RemoteConfiguration.MPTs);
				LocalConfiguration.Delays.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Delays.AddRange(RemoteConfiguration.Delays);
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
						if (sameObject1.ObjectType == ObjectType.PumpStation)
						{
							newObject.DifferenceDiscription = GetPumpStationsDifferences(sameObject1, sameObject2, IsLocalConfig);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.MPT)
						{
							newObject.DifferenceDiscription = GetMPTsDifferences(sameObject1, sameObject2, IsLocalConfig);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.Delay)
						{
							newObject.DifferenceDiscription = GetDelaysDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
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

		string GetZonesDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var zonesDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				zonesDifferences.Append("Не совпадает название");
			return zonesDifferences.ToString() == "" ? null : zonesDifferences.ToString();
		}

		string GetPumpStationsDifferences(ObjectViewModel object1, ObjectViewModel object2, bool isLocalConfig)
		{
			var pumpStationsDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				pumpStationsDifferences.Append("Не совпадает название");
			if (object1.PumpStation.NSDevices.Any(nsDevice => object2.PumpStation.NSDevices.All(x => new ObjectViewModel(x).Compare(new ObjectViewModel(x), new ObjectViewModel(nsDevice)) != 0)))
			{
				if (pumpStationsDifferences.Length != 0)
					pumpStationsDifferences.Append(". ");
				pumpStationsDifferences.Append("Не совпадает количество насосов");
			}
			bool startDiff = GKManager.GetPresentationLogic(object1.PumpStation.StartLogic) != GKManager.GetPresentationLogic(object2.PumpStation.StartLogic);
			bool stopDiff = GKManager.GetPresentationLogic(object1.PumpStation.StopLogic) != GKManager.GetPresentationLogic(object2.PumpStation.StopLogic);
			bool automaticDiff = GKManager.GetPresentationLogic(object1.PumpStation.AutomaticOffLogic) != GKManager.GetPresentationLogic(object2.PumpStation.AutomaticOffLogic);
			if (startDiff || stopDiff || automaticDiff)
			{
				if (pumpStationsDifferences.Length != 0)
					pumpStationsDifferences.Append(". ");
				pumpStationsDifferences.Append("Не совпадают следующие условия: ");
				var logics = new List<string>();
				if(startDiff)
					logics.Add("Запуска");
				if(stopDiff)
					logics.Add("Запрета пуска");
				if(automaticDiff)
					logics.Add("Отключения");
				pumpStationsDifferences.Append(String.Join(", ", logics));
			}
			bool delayDiff = object1.PumpStation.Delay != object2.PumpStation.Delay;
			bool holdDiff = object1.PumpStation.Hold != object2.PumpStation.Hold;
			bool nsPumpsCountDiff = object1.PumpStation.NSPumpsCount != object2.PumpStation.NSPumpsCount;
			bool nsDeltaTimeDiff = object1.PumpStation.NSDeltaTime != object2.PumpStation.NSDeltaTime;
			if (delayDiff || holdDiff || nsPumpsCountDiff || nsDeltaTimeDiff)
			{
				if (pumpStationsDifferences.Length != 0)
					pumpStationsDifferences.Append(". ");
				pumpStationsDifferences.Append("Не совпадают следующие параметры: ");
				var parameters = new List<string>();
				if (delayDiff)
					parameters.Add("Задержка");
				if (holdDiff)
					parameters.Add("Время тушения");
				if (nsPumpsCountDiff)
					parameters.Add("Количество основных насосов");
				if (nsDeltaTimeDiff)
					parameters.Add("Интервал разновременного пуска");
				pumpStationsDifferences.Append(String.Join(", ", parameters));
			}
			return pumpStationsDifferences.ToString() == "" ? null : pumpStationsDifferences.ToString();
		}

		string GetMPTsDifferences(ObjectViewModel object1, ObjectViewModel object2, bool isLocalConfig)
		{
			var mptsDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				mptsDifferences.Append("Не совпадает название");
			if (object1.MPT.MPTDevices.Select(x => x.Device).Any(nsDevice => object2.MPT.MPTDevices.Select(x => x.Device).All(x => new ObjectViewModel(x).Compare(new ObjectViewModel(x), new ObjectViewModel(nsDevice)) != 0)))
			{
				if (mptsDifferences.Length != 0)
					mptsDifferences.Append(". ");
				mptsDifferences.Append("Не совпадают устройства");
			}
			bool startDiff = GKManager.GetPresentationLogic(object1.MPT.StartLogic) != GKManager.GetPresentationLogic(object2.MPT.StartLogic);
			if (startDiff)
			{
				mptsDifferences.Append("Не совпадают условия включения");
			}
			bool stopDiff = GKManager.GetPresentationLogic(object1.MPT.StopLogic) != GKManager.GetPresentationLogic(object2.MPT.StopLogic);
			if (stopDiff)
			{
				mptsDifferences.Append("Не совпадают условия выключения");
			}
			bool suspendDiff = GKManager.GetPresentationLogic(object1.MPT.SuspendLogic) != GKManager.GetPresentationLogic(object2.MPT.SuspendLogic);
			if (suspendDiff)
			{
				mptsDifferences.Append("Не совпадают условия приостановки");
			}
			bool delayDiff = object1.MPT.Delay != object2.MPT.Delay;
			if (delayDiff)
			{
				mptsDifferences.Append("Не совпадают задержки");
			}
			return mptsDifferences.ToString() == "" ? null : mptsDifferences.ToString();
		}

		string GetDelaysDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var delaysDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				delaysDifferences.Append("Не совпадает название");
			bool delayDiff = object1.Delay.DelayTime != object2.Delay.DelayTime;
			bool holdDiff = object1.Delay.Hold != object2.Delay.Hold;
			bool regimeDiff = object1.Delay.DelayRegime != object2.Delay.DelayRegime;
			if (delayDiff || holdDiff || regimeDiff)
			{
				if (delaysDifferences.Length != 0)
					delaysDifferences.Append(". ");
				delaysDifferences.Append("Не совпадают следующие параметры: ");
				var parameters = new List<string>();
				if (delayDiff)
					parameters.Add("Задержка");
				if (holdDiff)
					parameters.Add("Удержание");
				if (regimeDiff)
					parameters.Add("Режим работы");
				delaysDifferences.Append(String.Join(", ", parameters));
			}
			return delaysDifferences.ToString() == "" ? null : delaysDifferences.ToString();
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
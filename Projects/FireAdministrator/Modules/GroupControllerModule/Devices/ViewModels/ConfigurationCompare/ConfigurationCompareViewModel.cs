using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Ionic.Zip;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class ConfigurationCompareViewModel : DialogViewModel
	{
		GKDevice LocalDevice { get; set; }
		GKDevice RemoteDevice { get; set; }
		GKDeviceConfiguration LocalConfiguration { get; set; }
		GKDeviceConfiguration RemoteConfiguration { get; set; }
		string ConfigFileName { get; set; }
		bool OnlyGKDeviceConfiguration { get; set; }
		public ObjectsListViewModel LocalObjectsViewModel { get; set; }
		public ObjectsListViewModel RemoteObjectsViewModel { get; set; }
		internal static bool ConfigFromFile { get; private set; }
		public string Error { get; private set; }
		public bool CanChangeOrOpenConfiguration { get; private set; }

		public ConfigurationCompareViewModel(GKDeviceConfiguration localConfiguration, GKDeviceConfiguration remoteConfiguration, GKDevice device, string configFileName = "")
		{
			Title = "Сравнение конфигураций " + device.PresentationName;
			ChangeCurrentGkCommand = new RelayCommand(OnChangeCurrentGk);
			LoadConfigurationFromFileCommand = new RelayCommand(OnLoadConfigurationFromFile);
			NextDifferenceCommand = new RelayCommand(OnNextDifference, CanNextDifference);
			PreviousDifferenceCommand = new RelayCommand(OnPreviousDifference, CanPreviousDifference);

			ConfigFileName = configFileName;
			ConfigFromFile = CanChangeOrOpenConfiguration = !string.IsNullOrEmpty(configFileName);

			var remoteConfig = new ZipFile(ConfigFileName);
			OnlyGKDeviceConfiguration = remoteConfig.Entries.Count == 1;

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

		public RelayCommand ChangeCurrentGkCommand { get; private set; }
		void OnChangeCurrentGk()
		{
			RemoteDevice.UID = LocalDevice.UID;
			var rootDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.UID == LocalDevice.Parent.UID);
			rootDevice.Children.Remove(LocalDevice);
			rootDevice.Children.Add(RemoteDevice);
			if (LocalDevice.DriverType == GKDriverType.GK)
			{
				LocalConfiguration.Zones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Zones.AddRange(RemoteConfiguration.Zones);
				LocalConfiguration.Directions.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Directions.AddRange(RemoteConfiguration.Directions);
				LocalConfiguration.PumpStations.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.PumpStations.AddRange(RemoteConfiguration.PumpStations);
				LocalConfiguration.MPTs.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.MPTs.AddRange(RemoteConfiguration.MPTs);
				LocalConfiguration.Delays.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Delays.AddRange(RemoteConfiguration.Delays);
				LocalConfiguration.GuardZones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.GuardZones.AddRange(RemoteConfiguration.GuardZones);
				LocalConfiguration.Codes.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Codes.AddRange(RemoteConfiguration.Codes);
				LocalConfiguration.Doors.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Doors.AddRange(RemoteConfiguration.Doors);
			}
			ServiceFactory.SaveService.GKChanged = true;
			GKManager.UpdateConfiguration();
			Close(true);
		}

		public RelayCommand LoadConfigurationFromFileCommand { get; private set; }
		void OnLoadConfigurationFromFile()
		{
			var result = true;
			if (OnlyGKDeviceConfiguration)
				result = MessageBoxService.ShowQuestion("Файл содержит только конфигурацию устройств. При его открытии будут утеряны макеты и планы.\nОткрыть файл?");
			if (result)
			{
				ServiceFactory.Events.GetEvent<LoadFromFileEvent>().Publish(ConfigFileName);
				Close(true);
			}
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
				//if (unionObjects.All(x => x.Compare(x, object2) != 0))
				if (unionObjects.All(x => x.SortingName != object2.SortingName))
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
				var sameObject1 = objects1.FirstOrDefault(x => IsEqual(unionObject, x));
				if (sameObject1 == null)
				{
					newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в локальной конфигурации" : "Отсутствует в конфигурации прибора";
					newObject.IsAbsent = true;
				}
				else
				{
					var sameObject2 = objects2.FirstOrDefault(x => IsEqual(unionObject, x));
					if (sameObject2 == null)
					{
						newObject.DifferenceDiscription = IsLocalConfig ? "Отсутствует в конфигурации прибора" : "Отсутствует в локальной конфигурации";
						newObject.IsPresent = true;
					}
					else
					{
						if (sameObject1.PresentationZoneOrLogic != sameObject2.PresentationZoneOrLogic)
						{
							if (sameObject1.IsDevice)
								newObject.DifferenceDiscription = sameObject1.Device.Driver.HasZone ? "Не совпадает зона" : "Не совпадает логика срабатывания";
							else
								newObject.DifferenceDiscription = "Не совпадает логика срабатывания";
						}
						newObject.PresentationZoneOrLogic = sameObject1.PresentationZoneOrLogic;
						if (sameObject1.ObjectType == ObjectType.Zone)
						{
							newObject.DifferenceDiscription = GetZonesDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.Direction)
						{
							newObject.DifferenceDiscription = GetDirectionsDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.PumpStation)
						{
							newObject.DifferenceDiscription = GetPumpStationsDifferences(sameObject1, sameObject2, IsLocalConfig);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.MPT)
						{
							newObject.DifferenceDiscription = GetMPTsDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.Delay)
						{
							newObject.DifferenceDiscription = GetDelaysDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.GuardZone)
						{
							newObject.DifferenceDiscription = GetGuardZonesDifferences(sameObject1, sameObject2);
							newObject.Name = sameObject1.Name;
						}
						if (sameObject1.ObjectType == ObjectType.Code)
						{
							newObject.DifferenceDiscription = GetCodesDifferences(sameObject1, sameObject2);
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
			var fire1CountDiff = object1.Zone.Fire1Count != object2.Zone.Fire1Count;
			var fire2CountDiff = object1.Zone.Fire2Count != object2.Zone.Fire2Count;
			if (fire1CountDiff || fire2CountDiff)
			{
				if (zonesDifferences.Length != 0)
					zonesDifferences.Append(". ");
				zonesDifferences.Append("Не совпадает число датчиков для формирования: ");
				var fires = new List<string>();
				if (fire1CountDiff)
					fires.Add("Пожар1");
				if (fire2CountDiff)
					fires.Add("Пожар2");
				zonesDifferences.Append(String.Join(", ", fires));
			}
			return zonesDifferences.ToString() == "" ? null : zonesDifferences.ToString();
		}

		string GetDirectionsDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var directionsDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				directionsDifferences.Append("Не совпадает название");
			bool delayDiff = object1.Direction.Delay != object2.Direction.Delay;
			bool holdDiff = object1.Direction.Hold != object2.Direction.Hold;
			bool regimeDiff = object1.Direction.DelayRegime != object2.Direction.DelayRegime;
			bool logicDiff = GKManager.GetPresentationLogic(object1.Direction.Logic) != GKManager.GetPresentationLogic(object2.Direction.Logic);
			if (delayDiff || holdDiff || regimeDiff || logicDiff)
			{
				if (directionsDifferences.Length != 0)
					directionsDifferences.Append(". ");
				directionsDifferences.Append("Не совпадают следующие параметры: ");
				var parameters = new List<string>();
				if (delayDiff)
					parameters.Add("Задержка");
				if (holdDiff)
					parameters.Add("Удержание");
				if (regimeDiff)
					parameters.Add("Режим работы");
				if (logicDiff)
					parameters.Add("Логика");
				directionsDifferences.Append(String.Join(", ", parameters));
			}
			return directionsDifferences.ToString() == "" ? null : directionsDifferences.ToString();
		}

		string GetPumpStationsDifferences(ObjectViewModel object1, ObjectViewModel object2, bool isLocalConfig)
		{
			var pumpStationsDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				pumpStationsDifferences.Append("Не совпадает название");
			if (object1.PumpStation.NSDevices.Any(nsDevice1 => object2.PumpStation.NSDevices.All(nsDevice2 => !IsEqualDevice(nsDevice1, nsDevice2)))
				|| object1.PumpStation.NSDevices.Count < object2.PumpStation.NSDevices.Count)
			{
				if (pumpStationsDifferences.Length != 0)
					pumpStationsDifferences.Append(". ");
				pumpStationsDifferences.Append("Не совпадают насосы");
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
				if (startDiff)
					logics.Add("Запуска");
				if (stopDiff)
					logics.Add("Запрета пуска");
				if (automaticDiff)
					logics.Add("Отключения");
				pumpStationsDifferences.Append(String.Join(", ", logics));
			}
			bool delayDiff = object1.PumpStation.Delay != object2.PumpStation.Delay;
			bool regimeDiff = object1.PumpStation.DelayRegime != object2.PumpStation.DelayRegime;
			bool holdDiff = object1.PumpStation.Hold != object2.PumpStation.Hold;
			bool nsPumpsCountDiff = object1.PumpStation.NSPumpsCount != object2.PumpStation.NSPumpsCount;
			bool nsDeltaTimeDiff = object1.PumpStation.NSDeltaTime != object2.PumpStation.NSDeltaTime;
			if (delayDiff || regimeDiff || holdDiff || nsPumpsCountDiff || nsDeltaTimeDiff)
			{
				if (pumpStationsDifferences.Length != 0)
					pumpStationsDifferences.Append(". ");
				pumpStationsDifferences.Append("Не совпадают следующие параметры: ");
				var parameters = new List<string>();
				if (delayDiff)
					parameters.Add("Задержка");
				if (regimeDiff)
					parameters.Add("Режим после удержания");
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

		string GetMPTsDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var mptsDifferences = new List<string>();
			if (object1.Name != object2.Name)
				mptsDifferences.Add("Не совпадает название");
			var devices1 = object1.MPT.MPTDevices.Select(x => x.Device);
			var devices2 = object2.MPT.MPTDevices.Select(x => x.Device);
			if (devices1.Any(nsDevice1 => devices2.All(nsDevice2 => !IsEqualDevice(nsDevice1, nsDevice2)))
				|| devices1.Count() < devices2.Count())
			{
				mptsDifferences.Add("Не совпадают устройства");
			}
			bool startDiff = GKManager.GetPresentationLogic(object1.MPT.MptLogic.OnClausesGroup) != GKManager.GetPresentationLogic(object2.MPT.MptLogic.OnClausesGroup);
			if (startDiff)
			{
				mptsDifferences.Add("Не совпадают условия включения");
			}
			bool stopDiff = GKManager.GetPresentationLogic(object1.MPT.MptLogic.OffClausesGroup) != GKManager.GetPresentationLogic(object2.MPT.MptLogic.OffClausesGroup);
			if (stopDiff)
			{
				mptsDifferences.Add("Не совпадают условия выключения");
			}
			bool suspendDiff = GKManager.GetPresentationLogic(object1.MPT.MptLogic.StopClausesGroup) != GKManager.GetPresentationLogic(object2.MPT.MptLogic.StopClausesGroup);
			if (suspendDiff)
			{
				mptsDifferences.Add("Не совпадают условия приостановки");
			}
			bool delayDiff = object1.MPT.Delay != object2.MPT.Delay;
			if (delayDiff)
			{
				mptsDifferences.Add("Не совпадают задержки");
			}
			return String.Join(". ", mptsDifferences);
		}

		string GetDelaysDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var delaysDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				delaysDifferences.Append("Не совпадает название");
			bool delayDiff = object1.Delay.DelayTime != object2.Delay.DelayTime;
			bool holdDiff = object1.Delay.Hold != object2.Delay.Hold;
			bool regimeDiff = object1.Delay.DelayRegime != object2.Delay.DelayRegime;
			bool logicDiff = GKManager.GetPresentationLogic(object1.Delay.Logic) != GKManager.GetPresentationLogic(object2.Delay.Logic);
			if (delayDiff || holdDiff || regimeDiff || logicDiff)
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
				if (logicDiff)
					parameters.Add("Логика");
				delaysDifferences.Append(String.Join(", ", parameters));
			}
			return delaysDifferences.ToString() == "" ? null : delaysDifferences.ToString();
		}

		string GetGuardZonesDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var guardZonesDifferences = new StringBuilder();
			if (object1.Name != object2.Name)
				guardZonesDifferences.Append("Не совпадает название");
			if (object1.GuardZone.SetDelay != object2.GuardZone.SetDelay)
			{
				if (guardZonesDifferences.Length != 0)
					guardZonesDifferences.Append(". ");
				guardZonesDifferences.Append("Не совпадает задержка");
			}
			return guardZonesDifferences.ToString() == "" ? null : guardZonesDifferences.ToString();
		}

		string GetCodesDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var differences = new List<string>();
			if (object1.Name != object2.Name)
				differences.Add("Не совпадает название");
			if (object1.Code.Password != object2.Code.Password)
			{
				differences.Add("Не совпадает пароль");
			}
			return string.Join(". ", differences);
		}


		string GetDoorsDifferences(ObjectViewModel object1, ObjectViewModel object2)
		{
			var differences = new List<string>();
			if (object1.Name != object2.Name)
				differences.Add("Не совпадает название");
			if (object1.Door.DoorType != object2.Door.DoorType)
			{
				differences.Add("Не совпадает тип");
			}

			if (!IsEqualDevice(object1.Door.EnterDevice, object2.Door.EnterDevice) || !IsEqualDevice(object1.Door.ExitDevice, object2.Door.ExitDevice)
				|| !IsEqualDevice(object1.Door.LockDevice, object2.Door.LockDevice) || !IsEqualDevice(object1.Door.LockDeviceExit, object2.Door.LockDeviceExit)
				|| !IsEqualDevice(object1.Door.LockControlDevice, object2.Door.LockControlDevice) || !IsEqualDevice(object1.Door.LockControlDeviceExit, object2.Door.LockControlDeviceExit))
			{
				differences.Add("Не совпадают устройства");
			}
			if (object1.Door.Delay != object2.Door.Delay)
			{
				differences.Add("Не совпадает задержка");
			}
			if (object1.Door.Hold != object2.Door.Hold)
			{
				differences.Add("Не совпадает удержание");
			}
			if (object1.Door.EnterZoneUID != object2.Door.EnterZoneUID)
			{
				differences.Add("Не совпадает зона входа");
			}
			if (object1.Door.ExitZoneUID != object2.Door.ExitZoneUID)
			{
				differences.Add("Не совпадает зона выхода");
			}
			bool openLogicDiff = GKManager.GetPresentationLogic(object1.Door.OpenRegimeLogic) != GKManager.GetPresentationLogic(object2.Door.OpenRegimeLogic);
			if (openLogicDiff)
			{
				differences.Add("Не совпадают условия перевода в режим Всегда Включено");
			}
			return string.Join(". ", differences);
		}
		bool IsEqual(ObjectViewModel viewModel1, ObjectViewModel viewModel2)
		{
			if (viewModel1.ObjectType != viewModel2.ObjectType)
				return false;

			if (viewModel1.ObjectType == ObjectType.Device)
			{
				return IsEqualDevice(viewModel1.Device, viewModel2.Device);
			}

			if (viewModel1.ObjectType == ObjectType.Zone)
				return viewModel1.Zone.No == viewModel2.Zone.No;

			if (viewModel1.ObjectType == ObjectType.Direction)
				return viewModel1.Direction.No == viewModel2.Direction.No;

			if (viewModel1.ObjectType == ObjectType.PumpStation)
				return viewModel1.PumpStation.No == viewModel2.PumpStation.No;

			if (viewModel1.ObjectType == ObjectType.MPT)
				return viewModel1.MPT.No == viewModel2.MPT.No;

			if (viewModel1.ObjectType == ObjectType.Delay)
				return viewModel1.Delay.No == viewModel2.Delay.No;

			if (viewModel1.ObjectType == ObjectType.GuardZone)
				return viewModel1.GuardZone.No == viewModel2.GuardZone.No;

			if (viewModel1.ObjectType == ObjectType.Code)
				return viewModel1.Code.No == viewModel2.Code.No;

			if (viewModel1.ObjectType == ObjectType.Door)
				return viewModel1.Door.No == viewModel2.Door.No;

			if (viewModel1.ObjectType == ObjectType.SKDZone)
				return viewModel1.SKDZone.No == viewModel2.SKDZone.No;

			return true;
		}
		bool IsEqualDevice(GKDevice device1, GKDevice device2)
		{
			if (device1 == null && device2 == null)
				return true;
			if (device1 == null && device2 != null)
				return false;
			if (device1 != null && device2 == null)
				return false;

			if (device1.DriverType == GKDriverType.GKIndicatorsGroup
			|| device1.DriverType == GKDriverType.GKIndicator
			|| device1.DriverType == GKDriverType.GKRelaysGroup
			|| device1.DriverType == GKDriverType.GKRele)
				return true;

			var kauIntAddress1 = device1.KAUParent != null ? device1.KAUParent.IntAddress : 0;
			var kauIntAddress2 = device2.KAUParent != null ? device2.KAUParent.IntAddress : 0;
			if (kauIntAddress1 != kauIntAddress2)
				return false;

			if (device1.ShleifNo != device2.ShleifNo)
				return false;

			if (device1.IntAddress != device2.IntAddress)
				return false;

			if (device1.Driver.DriverType != device2.Driver.DriverType)
				return false;
			return true;
		}
	}
}
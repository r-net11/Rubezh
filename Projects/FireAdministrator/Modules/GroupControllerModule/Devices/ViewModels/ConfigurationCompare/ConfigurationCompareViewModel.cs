using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
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
        public string Error { get; private set; }
		public ConfigurationCompareViewModel(XDeviceConfiguration localConfiguration, XDeviceConfiguration remoteConfiguration, XDevice device, bool configFromFile)
		{
			Title = "Сравнение конфигураций " + device.PresentationName;
			ConfigFromFile = configFromFile;
			ChangeCommand = new RelayCommand(OnChange);
			NextDifferenceCommand = new RelayCommand(OnNextDifference, CanNextDifference);
			PreviousDifferenceCommand = new RelayCommand(OnPreviousDifference, CanPreviousDifference);

			LocalConfiguration = localConfiguration;
			RemoteConfiguration = remoteConfiguration;
			RemoteConfiguration.Update();
			UpdateConfigurationHelper.Update(RemoteConfiguration);

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
			if (LocalDevice.DriverType == XDriverType.GK)
			{
				LocalConfiguration.Zones.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Zones.AddRange(RemoteConfiguration.Zones);
				LocalConfiguration.Directions.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.Directions.AddRange(RemoteConfiguration.Directions);
				LocalConfiguration.PumpStations.RemoveAll(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == LocalDevice.Address);
				LocalConfiguration.PumpStations.AddRange(RemoteConfiguration.PumpStations);
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
						if (sameObject1.PresentationZone != sameObject2.PresentationZone)
							newObject.DifferenceDiscription = sameObject1.Device.Driver.HasZone ? "Не совпадает зона" : "Не совпадает логика срабатывания";
						newObject.PresentationZone = sameObject1.PresentationZone;
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
		    if (delayDiff || holdDiff || regimeDiff)
		    {
                if (directionsDifferences.Length != 0)
                    directionsDifferences.Append(". ");
                directionsDifferences.Append("Не совпадают следующие параметры: ");
		        var parameters = new List<string>();
                if(delayDiff)
                    parameters.Add("Задержка");
                if (holdDiff)
                    parameters.Add("Удержание");
                if (regimeDiff)
                    parameters.Add("Режим работы");
		        directionsDifferences.Append(String.Join(", ", parameters));
		    }
		    return directionsDifferences.ToString() == "" ? null : directionsDifferences.ToString();
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
            bool startDiff = XManager.GetPresentationZone(object1.PumpStation.StartLogic) != XManager.GetPresentationZone(object2.PumpStation.StartLogic);
            bool stopDiff = XManager.GetPresentationZone(object1.PumpStation.StopLogic) != XManager.GetPresentationZone(object2.PumpStation.StopLogic);
            bool automaticDiff = XManager.GetPresentationZone(object1.PumpStation.AutomaticOffLogic) != XManager.GetPresentationZone(object2.PumpStation.AutomaticOffLogic);
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
	}
}
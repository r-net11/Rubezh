using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Devices;

namespace DeviceControls.Device
{
	public class DevicePicture : BaseDevicePicture<LibraryState, LibraryFrame, StateType, DeviceState>
	{
		private Dictionary<Guid, string> _driverPresenterMap;

		public DevicePicture()
		{
			_driverPresenterMap = new Dictionary<Guid, string>();
		}

		public Brush GetBrush(FiresecAPI.Models.Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public Brush GetDynamicBrush(FiresecAPI.Models.Device device, Guid alternativeDriverUID)
		{
			var presenterKey = GetPresenterKey(device);
			return device == null || device.DriverUID == Guid.Empty || device.DeviceState == null ? GetBrush(device) : GetDynamicBrush(alternativeDriverUID == Guid.Empty ? device.DriverUID : alternativeDriverUID, presenterKey, device.DeviceState);
		}

		public override void LoadDynamicCache()
		{
			base.LoadDynamicCache();
			_driverPresenterMap.Clear();
			FiresecManager.Drivers.ForEach(driver => _driverPresenterMap.Add(driver.UID, driver.PresenterKeyPropertyName));
		}

		protected override IEnumerable<ILibraryDevice<LibraryState, LibraryFrame, StateType>> EnumerateLibrary()
		{
			foreach (var device in FiresecManager.DeviceLibraryConfiguration.Devices)
				yield return device;
		}
		protected override StateType DefaultState
		{
			get { return StateType.No; }
		}

		protected override void AddAdditionalDynamicBrushes(ILibraryDevice<LibraryState, LibraryFrame, StateType> item)
		{
			var libraryDevice = (LibraryDevice)item;
			if (libraryDevice.Presenters != null)
				libraryDevice.Presenters.ForEach(presenter =>
				{
					if (!DynamicBrushes[libraryDevice.DriverId].ContainsKey(presenter.Key))
						DynamicBrushes[libraryDevice.DriverId].Add(presenter.Key, new Dictionary<StateType, Dictionary<string, Brush>>());
					presenter.States.ForEach(state =>
					{
						if (!DynamicBrushes[libraryDevice.DriverId][presenter.Key].ContainsKey(state.StateType))
							DynamicBrushes[libraryDevice.DriverId][presenter.Key].Add(state.StateType, new Dictionary<string, Brush>());
						if (!DynamicBrushes[libraryDevice.DriverId][presenter.Key][state.StateType].ContainsKey(state.Code ?? string.Empty))
							DynamicBrushes[libraryDevice.DriverId][presenter.Key][state.StateType].Add(state.Code ?? string.Empty, PictureCacheSource.CreateDynamicBrush(state.Frames));
					});
				});
		}
		protected override Brush GetDynamicBrush(Dictionary<StateType, Dictionary<string, Brush>> map, DeviceState deviceState)
		{
			var brushes = map.ContainsKey(deviceState.StateType) ? map[deviceState.StateType] : null;
			var brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			if (brushes != null)
				foreach (var state in deviceState.ThreadSafeStates)
					if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
					{
						brush = brushes[state.DriverState.Code];
						break;
					}
			if (brush == null && brushes != null)
				brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			if (brush == null && map.ContainsKey(StateType.No))
			{
				brushes = map[StateType.No];
				foreach (var state in deviceState.ThreadSafeStates)
					if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
					{
						brush = brushes[state.DriverState.Code];
						break;
					}
				if (brush == null && brushes != null)
					brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			}
			return brush;
		}

		private string GetPresenterKey(FiresecAPI.Models.Device device)
		{
			if (_driverPresenterMap.ContainsKey(device.DriverUID) && !string.IsNullOrEmpty(_driverPresenterMap[device.DriverUID]))
				return device.Properties.Where(prop => prop.Name == _driverPresenterMap[device.DriverUID]).Select(prop => prop.Value).FirstOrDefault() ?? string.Empty;
			else
				return string.Empty;
		}
	}
}
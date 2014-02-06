using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

namespace DeviceControls.Device
{
	public class DevicePicture
	{
		private Dictionary<Guid, Brush> _brushes = new Dictionary<Guid, Brush>();
		private Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>> _dynamicBrushes = new Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>>();
		private Dictionary<Guid, string> _driverPresenterMap = new Dictionary<Guid, string>();

		internal DevicePicture()
		{
		}

		public void LoadCache()
		{
			_brushes.Clear();
			RegisterBrush(null);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterBrush(item));
		}
		public void LoadDynamicCache()
		{
			_dynamicBrushes.Clear();
			_dynamicBrushes.Add(Guid.Empty, new Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>());
			_dynamicBrushes[Guid.Empty].Add(string.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
			_dynamicBrushes[Guid.Empty][string.Empty].Add(StateType.No, new Dictionary<string, Brush>());
			_dynamicBrushes[Guid.Empty][string.Empty][StateType.No].Add(string.Empty, PictureCacheSource.EmptyBrush);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item =>
			{
				if (!_dynamicBrushes.ContainsKey(item.DriverId))
				{
					_dynamicBrushes.Add(item.DriverId, new Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>());
					_dynamicBrushes[item.DriverId].Add(string.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
				}
				item.States.ForEach(state =>
				{
					if (!_dynamicBrushes[item.DriverId][string.Empty].ContainsKey(state.StateType))
						_dynamicBrushes[item.DriverId][string.Empty].Add(state.StateType, new Dictionary<string, Brush>());
					if (!_dynamicBrushes[item.DriverId][string.Empty][state.StateType].ContainsKey(state.Code ?? string.Empty))
						_dynamicBrushes[item.DriverId][string.Empty][state.StateType].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.Frames));
				});
				if (item.Presenters != null)
					item.Presenters.ForEach(presenter =>
					{
						if (!_dynamicBrushes[item.DriverId].ContainsKey(presenter.Key))
							_dynamicBrushes[item.DriverId].Add(presenter.Key, new Dictionary<StateType, Dictionary<string, Brush>>());
						presenter.States.ForEach(state =>
						{
							if (!_dynamicBrushes[item.DriverId][presenter.Key].ContainsKey(state.StateType))
								_dynamicBrushes[item.DriverId][presenter.Key].Add(state.StateType, new Dictionary<string, Brush>());
							if (!_dynamicBrushes[item.DriverId][presenter.Key][state.StateType].ContainsKey(state.Code ?? string.Empty))
								_dynamicBrushes[item.DriverId][presenter.Key][state.StateType].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.Frames));
						});
					});
			});
			_driverPresenterMap.Clear();
			FiresecManager.Drivers.ForEach(driver => _driverPresenterMap.Add(driver.UID, driver.PresenterKeyPropertyName));
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
		public Brush CreatePreviewBrush(List<LibraryFrame> frames)
		{
			return CreateDynamicBrush(frames);
		}

		private void RegisterBrush(LibraryDevice libraryDevice)
		{
			var frameworkElement = libraryDevice == null ? PictureCacheSource.EmptyPicture : GetDefaultPicture(libraryDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_brushes.ContainsKey(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId))
				_brushes[libraryDevice == null ? Guid.Empty : libraryDevice.DriverId] = brush;
			else
				_brushes.Add(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId, brush);
		}
		private Brush CreateDynamicBrush(List<LibraryFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}

		private Brush GetBrush(Guid driverUID)
		{
			if (!_brushes.ContainsKey(driverUID))
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!_brushes.ContainsKey(Guid.Empty))
						RegisterBrush(null);
					return _brushes[Guid.Empty];
				}
				else
					RegisterBrush(libraryDevice);
			}
			return _brushes[driverUID];
		}
		private Brush GetDynamicBrush(Guid guid, string presenterKey, DeviceState deviceState)
		{
			Brush brush = null;
			try
			{
				if (_dynamicBrushes.ContainsKey(guid))
				{
					if (!string.IsNullOrEmpty(presenterKey) && _dynamicBrushes[guid].ContainsKey(presenterKey))
						brush = GetDynamicBrush(_dynamicBrushes[guid][presenterKey], deviceState);
					if (brush == null)
						brush = GetDynamicBrush(_dynamicBrushes[guid][string.Empty], deviceState);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DevicePictureCache.GetDynamicBrush(Guid guid, DeviceState deviceState)");
			}
			return brush ?? PictureCacheSource.EmptyBrush;
		}

		private FrameworkElement GetDefaultPicture(LibraryDevice device)
		{
			var state = device.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : PictureCacheSource.EmptyPicture;
		}

		private string GetPresenterKey(FiresecAPI.Models.Device device)
		{
			if (_driverPresenterMap.ContainsKey(device.DriverUID) && !string.IsNullOrEmpty(_driverPresenterMap[device.DriverUID]))
				return device.Properties.Where(prop => prop.Name == _driverPresenterMap[device.DriverUID]).Select(prop => prop.Value).FirstOrDefault() ?? string.Empty;
			else
				return string.Empty;
		}
		private Brush GetDynamicBrush(Dictionary<StateType, Dictionary<string, Brush>> map, DeviceState deviceState)
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
	}
	//public class DevicePictureCache2 : BaseDevicePicture<LibraryFrame>
	//{
	//    private Dictionary<Guid, string> _driverPresenterMap = new Dictionary<Guid, string>();
	//    private Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>> _dynamicBrushes = new Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>>();

	//    public override void LoadDynamicCache()
	//    {
	//        _dynamicBrushes.Clear();
	//        _dynamicBrushes.Add(Guid.Empty, new Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>());
	//        _dynamicBrushes[Guid.Empty].Add(string.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
	//        _dynamicBrushes[Guid.Empty][string.Empty].Add(StateType.No, new Dictionary<string, Brush>());
	//        _dynamicBrushes[Guid.Empty][string.Empty][StateType.No].Add(string.Empty, DevicePictureCache.EmptyBrush);
	//        FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item =>
	//        {
	//            if (!_dynamicBrushes.ContainsKey(item.DriverId))
	//            {
	//                _dynamicBrushes.Add(item.DriverId, new Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>());
	//                _dynamicBrushes[item.DriverId].Add(string.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
	//            }
	//            item.States.ForEach(state =>
	//            {
	//                if (!_dynamicBrushes[item.DriverId][string.Empty].ContainsKey(state.StateType))
	//                    _dynamicBrushes[item.DriverId][string.Empty].Add(state.StateType, new Dictionary<string, Brush>());
	//                if (!_dynamicBrushes[item.DriverId][string.Empty][state.StateType].ContainsKey(state.Code ?? string.Empty))
	//                    _dynamicBrushes[item.DriverId][string.Empty][state.StateType].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.Frames));
	//            });
	//            if (item.Presenters != null)
	//                item.Presenters.ForEach(presenter =>
	//                {
	//                    if (!_dynamicBrushes[item.DriverId].ContainsKey(presenter.Key))
	//                        _dynamicBrushes[item.DriverId].Add(presenter.Key, new Dictionary<StateType, Dictionary<string, Brush>>());
	//                    presenter.States.ForEach(state =>
	//                    {
	//                        if (!_dynamicBrushes[item.DriverId][presenter.Key].ContainsKey(state.StateType))
	//                            _dynamicBrushes[item.DriverId][presenter.Key].Add(state.StateType, new Dictionary<string, Brush>());
	//                        if (!_dynamicBrushes[item.DriverId][presenter.Key][state.StateType].ContainsKey(state.Code ?? string.Empty))
	//                            _dynamicBrushes[item.DriverId][presenter.Key][state.StateType].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.Frames));
	//                    });
	//                });
	//        });
	//        _driverPresenterMap.Clear();
	//        FiresecManager.Drivers.ForEach(driver => _driverPresenterMap.Add(driver.UID, driver.PresenterKeyPropertyName));
	//    }
	//    public Brush GetBrush(FiresecAPI.Models.Device device)
	//    {
	//        Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
	//        return GetBrush(driverUID);
	//    }
	//    public Brush GetDynamicBrush(FiresecAPI.Models.Device device, Guid alternativeDriverUID)
	//    {
	//        var presenterKey = GetPresenterKey(device);
	//        return device == null || device.DriverUID == Guid.Empty || device.DeviceState == null ? GetBrush(device) : GetDynamicBrush(alternativeDriverUID == Guid.Empty ? device.DriverUID : alternativeDriverUID, presenterKey, device.DeviceState);
	//    }

	//    public Brush CreatePreviewBrush(List<LibraryFrame> frames)
	//    {
	//        return CreateDynamicBrush(frames);
	//    }

	//    private string GetPresenterKey(FiresecAPI.Models.Device device)
	//    {
	//        if (_driverPresenterMap.ContainsKey(device.DriverUID) && !string.IsNullOrEmpty(_driverPresenterMap[device.DriverUID]))
	//            return device.Properties.Where(prop => prop.Name == _driverPresenterMap[device.DriverUID]).Select(prop => prop.Value).FirstOrDefault() ?? string.Empty;
	//        else
	//            return string.Empty;
	//    }
	//    private Brush GetDynamicBrush(Guid guid, string presenterKey, DeviceState deviceState)
	//    {
	//        Brush brush = null;
	//        try
	//        {
	//            if (_dynamicBrushes.ContainsKey(guid))
	//            {
	//                if (!string.IsNullOrEmpty(presenterKey) && _dynamicBrushes[guid].ContainsKey(presenterKey))
	//                    brush = GetDynamicBrush(_dynamicBrushes[guid][presenterKey], deviceState);
	//                if (brush == null)
	//                    brush = GetDynamicBrush(_dynamicBrushes[guid][string.Empty], deviceState);
	//            }
	//        }
	//        catch (Exception e)
	//        {
	//            Logger.Error(e, "DevicePictureCache.GetDynamicBrush(Guid guid, DeviceState deviceState)");
	//        }
	//        return brush ?? DevicePictureCache.EmptyBrush;
	//    }
	//    private Brush GetDynamicBrush(Dictionary<StateType, Dictionary<string, Brush>> map, DeviceState deviceState)
	//    {
	//        var brushes = map.ContainsKey(deviceState.StateType) ? map[deviceState.StateType] : null;
	//        var brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
	//        if (brushes != null)
	//            foreach (var state in deviceState.ThreadSafeStates)
	//                if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
	//                {
	//                    brush = brushes[state.DriverState.Code];
	//                    break;
	//                }
	//        if (brush == null && brushes != null)
	//            brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
	//        if (brush == null && map.ContainsKey(StateType.No))
	//        {
	//            brushes = map[StateType.No];
	//            foreach (var state in deviceState.ThreadSafeStates)
	//                if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
	//                {
	//                    brush = brushes[state.DriverState.Code];
	//                    break;
	//                }
	//            if (brush == null && brushes != null)
	//                brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
	//        }
	//        return brush;
	//    }

	//    protected override ILibraryState<LibraryFrame> GetDefaultState(ILibraryDevice<LibraryFrame> libraryrDevice)
	//    {
	//        //return libraryrDevice.GetStates().FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
	//        return null;
	//    }
	//    protected override IEnumerable<ILibraryDevice<LibraryFrame>> EnumerateLibrary()
	//    {
	//        //return FiresecManager.DeviceLibraryConfiguration.Devices;
	//        return null;
	//    }
	//}
}
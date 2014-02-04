using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using XFiresecAPI;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public static class DevicePictureCache
	{
		private static Dictionary<Guid, Brush> _brushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Guid, Brush> _xbrushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Guid, Brush> _skdBrushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>> _dynamicBrushes = new Dictionary<Guid, Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>>();
		private static Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>> _dynamicXBrushes = new Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>>();
		private static Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>> _dynamicSKDBrushes = new Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>>();
		private static Dictionary<Guid, string> _driverPresenterMap = new Dictionary<Guid, string>();

		public static FrameworkElement EmptyPicture { get; private set; }
		public static Brush EmptyBrush { get; private set; }

		static DevicePictureCache()
		{
			EmptyPicture = new TextBlock()
			{
				Text = "?",
				Background = Brushes.Transparent,
				SnapsToDevicePixels = false
			};
			EmptyBrush = new VisualBrush(EmptyPicture);
		}
		public static void LoadCache()
		{
			_brushes.Clear();
			RegisterBrush(null);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterBrush(item));
		}
		public static void LoadXCache()
		{
			_brushes.Clear();
			RegisterXBrush(null);
			XManager.DeviceLibraryConfiguration.XDevices.ForEach(item => RegisterXBrush(item));
		}
		public static void LoadSKDCache()
		{
			_skdBrushes.Clear();
			RegisterSKDBrush(null);
			SKDManager.SKDLibraryConfiguration.Devices.ForEach(item => RegisterSKDBrush(item));
		}
		public static void LoadDynamicCache()
		{
			_dynamicBrushes.Clear();
			_dynamicBrushes.Add(Guid.Empty, new Dictionary<string, Dictionary<StateType, Dictionary<string, Brush>>>());
			_dynamicBrushes[Guid.Empty].Add(string.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
			_dynamicBrushes[Guid.Empty][string.Empty].Add(StateType.No, new Dictionary<string, Brush>());
			_dynamicBrushes[Guid.Empty][string.Empty][StateType.No].Add(string.Empty, EmptyBrush);
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
		public static void LoadXDynamicCache()
		{
			_dynamicXBrushes.Clear();
			_dynamicXBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Dictionary<string, Brush>>());
			_dynamicXBrushes[Guid.Empty].Add(XStateClass.No, new Dictionary<string, Brush>());
			_dynamicXBrushes[Guid.Empty][XStateClass.No].Add(string.Empty, EmptyBrush);
			XManager.DeviceLibraryConfiguration.XDevices.ForEach(item =>
			{
				if (!_dynamicXBrushes.ContainsKey(item.XDriverId))
					_dynamicXBrushes.Add(item.XDriverId, new Dictionary<XStateClass, Dictionary<string, Brush>>());
				item.XStates.ForEach(state =>
				{
					if (!_dynamicXBrushes[item.XDriverId].ContainsKey(state.XStateClass))
						_dynamicXBrushes[item.XDriverId].Add(state.XStateClass, new Dictionary<string, Brush>());
					if (!_dynamicXBrushes[item.XDriverId][state.XStateClass].ContainsKey(state.Code ?? string.Empty))
						_dynamicXBrushes[item.XDriverId][state.XStateClass].Add(state.Code ?? string.Empty, CreateDynamicXBrush(state.XFrames));
				});
			});
		}
		public static void LoadSKDDynamicCache()
		{
			_dynamicSKDBrushes.Clear();
			_dynamicSKDBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Dictionary<string, Brush>>());
			_dynamicSKDBrushes[Guid.Empty].Add(XStateClass.No, new Dictionary<string, Brush>());
			_dynamicSKDBrushes[Guid.Empty][XStateClass.No].Add(string.Empty, EmptyBrush);
			SKDManager.SKDLibraryConfiguration.Devices.ForEach(item =>
			{
				if (!_dynamicSKDBrushes.ContainsKey(item.DriverId))
					_dynamicSKDBrushes.Add(item.DriverId, new Dictionary<XStateClass, Dictionary<string, Brush>>());
				item.States.ForEach(state =>
				{
					if (!_dynamicSKDBrushes[item.DriverId].ContainsKey(state.StateClass))
						_dynamicSKDBrushes[item.DriverId].Add(state.StateClass, new Dictionary<string, Brush>());
					if (!_dynamicSKDBrushes[item.DriverId][state.StateClass].ContainsKey(state.Code ?? string.Empty))
						_dynamicSKDBrushes[item.DriverId][state.StateClass].Add(state.Code ?? string.Empty, CreateDynamicSKDBrush(state.Frames));
				});
			});
		}

		private static void RegisterBrush(LibraryDevice libraryDevice)
		{
			var frameworkElement = libraryDevice == null ? EmptyPicture : GetDefaultPicture(libraryDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_brushes.ContainsKey(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId))
				_brushes[libraryDevice == null ? Guid.Empty : libraryDevice.DriverId] = brush;
			else
				_brushes.Add(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId, brush);
		}
		private static void RegisterXBrush(LibraryXDevice libraryXDevice)
		{
			var frameworkElement = libraryXDevice == null ? EmptyPicture : GetDefaultXPicture(libraryXDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_xbrushes.ContainsKey(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId))
				_xbrushes[libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId] = brush;
			else
				_xbrushes.Add(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId, brush);
		}
		private static void RegisterSKDBrush(SKDLibraryDevice librarySKDDevice)
		{
			var frameworkElement = librarySKDDevice == null ? EmptyPicture : GetDefaultSKDPicture(librarySKDDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_skdBrushes.ContainsKey(librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId))
				_skdBrushes[librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId] = brush;
			else
				_skdBrushes.Add(librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId, brush);
		}
		private static Brush CreateDynamicBrush(List<LibraryFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}
		private static Brush CreateDynamicXBrush(List<LibraryXFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}
		private static Brush CreateDynamicSKDBrush(List<SKDLibraryFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}

		public static Brush GetBrush(FiresecAPI.Models.Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public static Brush GetXBrush(XFiresecAPI.XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetXBrush(driverUID);
		}
		public static Brush GetSKDBrush(FiresecAPI.SKDDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetSKDBrush(driverUID);
		}
		private static Brush GetBrush(Guid driverUID)
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
		private static Brush GetXBrush(Guid driverUID)
		{
			if (!_xbrushes.ContainsKey(driverUID))
			{
				var libraryDevice = XManager.DeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!_xbrushes.ContainsKey(Guid.Empty))
						RegisterXBrush(null);
					return _xbrushes[Guid.Empty];
				}
				else
					RegisterXBrush(libraryDevice);
			}
			return _xbrushes[driverUID];
		}
		private static Brush GetSKDBrush(Guid driverUID)
		{
			if (!_skdBrushes.ContainsKey(driverUID))
			{
				var libraryDevice = SKDManager.SKDLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!_skdBrushes.ContainsKey(Guid.Empty))
						RegisterSKDBrush(null);
					return _skdBrushes[Guid.Empty];
				}
				else
					RegisterSKDBrush(libraryDevice);
			}
			return _skdBrushes[driverUID];
		}

		public static Brush GetDynamicBrush(FiresecAPI.Models.Device device, Guid alternativeDriverUID)
		{
			var presenterKey = GetPresenterKey(device);
			return device == null || device.DriverUID == Guid.Empty || device.DeviceState == null ? GetBrush(device) : GetDynamicBrush(alternativeDriverUID == Guid.Empty ? device.DriverUID : alternativeDriverUID, presenterKey, device.DeviceState);
		}
		public static Brush GetDynamicXBrush(XFiresecAPI.XDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetXBrush(device) : GetDynamicXBrush(device.DriverUID, device.State);
		}
		public static Brush GetDynamicSKDBrush(FiresecAPI.SKDDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetSKDBrush(device) : GetDynamicSKDBrush(device.DriverUID, device.State);
		}
		private static Brush GetDynamicBrush(Guid guid, string presenterKey, DeviceState deviceState)
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
			return brush ?? EmptyBrush;
		}
		private static Brush GetDynamicXBrush(Guid guid, XState deviceState)
		{
			Brush brush = null;
			if (_dynamicXBrushes.ContainsKey(guid))
			{
				var brushes = _dynamicXBrushes[guid].ContainsKey(deviceState.StateClass) ? _dynamicXBrushes[guid][deviceState.StateClass] : null;
				brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
				if (brush == null && _dynamicXBrushes[guid].ContainsKey(XStateClass.No))
				{
					brushes = _dynamicXBrushes[guid][XStateClass.No];
					brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
				}
			}
			return brush ?? EmptyBrush;
		}
		private static Brush GetDynamicSKDBrush(Guid guid, SKDDeviceState deviceState)
		{
			Brush brush = null;
			if (_dynamicSKDBrushes.ContainsKey(guid))
			{
				var brushes = _dynamicSKDBrushes[guid].ContainsKey(deviceState.StateClass) ? _dynamicSKDBrushes[guid][deviceState.StateClass] : null;
				brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
				if (brush == null && _dynamicSKDBrushes[guid].ContainsKey(XStateClass.No))
				{
					brushes = _dynamicSKDBrushes[guid][XStateClass.No];
					brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
				}
			}
			return brush ?? EmptyBrush;
		}

		private static FrameworkElement GetDefaultPicture(LibraryDevice device)
		{
			var state = device.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : EmptyPicture;
		}
		private static FrameworkElement GetDefaultXPicture(LibraryXDevice device)
		{
			var state = device.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
			return state.XFrames.Count > 0 ? Helper.GetVisual(state.XFrames[0].Image) : EmptyPicture;
		}
		private static FrameworkElement GetDefaultSKDPicture(SKDLibraryDevice device)
		{
			var state = device.States.FirstOrDefault(x => x.Code == null && x.StateClass == XStateClass.No);
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : EmptyPicture;
		}

		private static string GetPresenterKey(FiresecAPI.Models.Device device)
		{
			if (_driverPresenterMap.ContainsKey(device.DriverUID) && !string.IsNullOrEmpty(_driverPresenterMap[device.DriverUID]))
				return device.Properties.Where(prop => prop.Name == _driverPresenterMap[device.DriverUID]).Select(prop => prop.Value).FirstOrDefault() ?? string.Empty;
			else
				return string.Empty;
		}
		private static Brush GetDynamicBrush(Dictionary<StateType, Dictionary<string, Brush>> map, DeviceState deviceState)
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

		public static Brush CreatePreviewBrush(List<LibraryFrame> frames)
		{
			return CreateDynamicBrush(frames);
		}
	}
}
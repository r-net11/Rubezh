using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Designer;
using XFiresecAPI;
using FiresecAPI;
using System.Windows.Controls;

namespace DeviceControls
{
	public static class DevicePictureCache
	{
		private static Dictionary<Guid, Brush> _brushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Guid, Brush> _xbrushes = new Dictionary<Guid, Brush>();
		private static Dictionary<Guid, Dictionary<StateType, Dictionary<string, Brush>>> _dynamicBrushes = new Dictionary<Guid, Dictionary<StateType, Dictionary<string, Brush>>>();
		private static Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>> _dynamicXBrushes = new Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>>();

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
			XManager.XDeviceLibraryConfiguration.XDevices.ForEach(item => RegisterXBrush(item));
		}
		public static void LoadDynamicCache()
		{
			_dynamicBrushes.Clear();
			_dynamicBrushes.Add(Guid.Empty, new Dictionary<StateType, Dictionary<string, Brush>>());
			_dynamicBrushes[Guid.Empty].Add(StateType.No, new Dictionary<string, Brush>());
			_dynamicBrushes[Guid.Empty][StateType.No].Add(string.Empty, EmptyBrush);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item =>
			{
				if (!_dynamicBrushes.ContainsKey(item.DriverId))
					_dynamicBrushes.Add(item.DriverId, new Dictionary<StateType, Dictionary<string, Brush>>());
				item.States.ForEach(state =>
				{
					if (!_dynamicBrushes[item.DriverId].ContainsKey(state.StateType))
						_dynamicBrushes[item.DriverId].Add(state.StateType, new Dictionary<string, Brush>());
					if (!_dynamicBrushes[item.DriverId][state.StateType].ContainsKey(state.Code ?? string.Empty))
						_dynamicBrushes[item.DriverId][state.StateType].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.Frames));
				});
			});
		}
		public static void LoadXDynamicCache()
		{
			_dynamicXBrushes.Clear();
			_dynamicXBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Dictionary<string, Brush>>());
			_dynamicXBrushes[Guid.Empty].Add(XStateClass.No, new Dictionary<string, Brush>());
			_dynamicXBrushes[Guid.Empty][XStateClass.No].Add(string.Empty, EmptyBrush);
			XManager.XDeviceLibraryConfiguration.XDevices.ForEach(item =>
			{
				if (!_dynamicXBrushes.ContainsKey(item.XDriverId))
					_dynamicXBrushes.Add(item.XDriverId, new Dictionary<XStateClass, Dictionary<string, Brush>>());
				item.XStates.ForEach(state =>
				{
					if (!_dynamicXBrushes[item.XDriverId].ContainsKey(state.XStateClass))
						_dynamicXBrushes[item.XDriverId].Add(state.XStateClass, new Dictionary<string, Brush>());
					if (!_dynamicXBrushes[item.XDriverId][state.XStateClass].ContainsKey(state.Code ?? string.Empty))
						_dynamicXBrushes[item.XDriverId][state.XStateClass].Add(state.Code ?? string.Empty, CreateDynamicBrush(state.XFrames));
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
			var frameworkElement = libraryXDevice == null ? EmptyPicture : GetDefaultPicture(libraryXDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_xbrushes.ContainsKey(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId))
				_xbrushes[libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId] = brush;
			else
				_xbrushes.Add(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId, brush);
		}
		private static Brush CreateDynamicBrush(List<LibraryFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}
		private static Brush CreateDynamicBrush(List<LibraryXFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}

		public static Brush GetBrush(Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public static Brush GetXBrush(XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetXBrush(driverUID);
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
				var libraryDevice = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == driverUID);
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

		public static Brush GetDynamicBrush(Device device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.DeviceState == null ? GetBrush(device) : GetDynamicBrush(device.DriverUID, device.DeviceState);
		}
		public static Brush GetDynamicBrush(XDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.DeviceState == null ? GetXBrush(device) : GetDynamicBrush(device.DriverUID, device.DeviceState);
		}
		private static Brush GetDynamicBrush(Guid guid, DeviceState deviceState)
		{
			Brush brush = null;
            try
            {
                if (_dynamicBrushes.ContainsKey(guid))
                {
                    var brushes = _dynamicBrushes[guid].ContainsKey(deviceState.StateType) ? _dynamicBrushes[guid][deviceState.StateType] : null;
                    brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
                    if (brushes != null)
                        foreach (var state in deviceState.ThreadSafeStates)
                            if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
                            {
                                brush = brushes[state.DriverState.Code];
                                break;
                            }
                    if (brush == null && brushes != null)
                    {
                        brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
                        if (_dynamicBrushes[guid].ContainsKey(StateType.No))
                        {
                            brushes = _dynamicBrushes[guid][StateType.No];
                            foreach (var state in deviceState.ThreadSafeStates)
                                if (state.DriverState.StateType == deviceState.StateType && brushes.ContainsKey(state.DriverState.Code))
                                {
                                    brush = brushes[state.DriverState.Code];
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "DevicePictureCache.GetDynamicBrush(Guid guid, DeviceState deviceState)");
            }
			return brush ?? EmptyBrush;
		}
		private static Brush GetDynamicBrush(Guid guid, XDeviceState deviceState)
		{
			Brush brush = null;
			if (_dynamicBrushes.ContainsKey(guid))
			{
				var brushes = _dynamicXBrushes[guid].ContainsKey(deviceState.StateClass) ? _dynamicXBrushes[guid][deviceState.StateClass] : null;
				brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
				if (brush == null && _dynamicXBrushes[guid].ContainsKey(XStateClass.No))
					brush = brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			}
			return brush ?? EmptyBrush;
		}

		private static FrameworkElement GetDefaultPicture(LibraryDevice device)
		{
			var state = device.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : EmptyPicture;
		}
		private static FrameworkElement GetDefaultPicture(LibraryXDevice device)
		{
			var state = device.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
			return state.XFrames.Count > 0 ? Helper.GetVisual(state.XFrames[0].Image) : EmptyPicture;
		}
	}
}
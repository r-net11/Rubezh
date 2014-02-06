using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using FiresecAPI;
using XFiresecAPI;

namespace DeviceControls.SKDDevice
{
	public class SKDDevicePicture
	{
		private Dictionary<Guid, Brush> _skdBrushes = new Dictionary<Guid, Brush>();
		private Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>> _dynamicSKDBrushes = new Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>>();

		internal SKDDevicePicture()
		{
		}
	
		public void LoadSKDCache()
		{
			_skdBrushes.Clear();
			RegisterSKDBrush(null);
			SKDManager.SKDLibraryConfiguration.Devices.ForEach(item => RegisterSKDBrush(item));
		}
		public void LoadSKDDynamicCache()
		{
			_dynamicSKDBrushes.Clear();
			_dynamicSKDBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Dictionary<string, Brush>>());
			_dynamicSKDBrushes[Guid.Empty].Add(XStateClass.No, new Dictionary<string, Brush>());
			_dynamicSKDBrushes[Guid.Empty][XStateClass.No].Add(string.Empty, PictureCacheSource.EmptyBrush);
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

		private void RegisterSKDBrush(SKDLibraryDevice librarySKDDevice)
		{
			var frameworkElement = librarySKDDevice == null ? PictureCacheSource.EmptyPicture : GetDefaultSKDPicture(librarySKDDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_skdBrushes.ContainsKey(librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId))
				_skdBrushes[librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId] = brush;
			else
				_skdBrushes.Add(librarySKDDevice == null ? Guid.Empty : librarySKDDevice.DriverId, brush);
		}
		private Brush CreateDynamicSKDBrush(List<SKDLibraryFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}

		public Brush GetSKDBrush(FiresecAPI.SKDDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetSKDBrush(driverUID);
		}
		private Brush GetSKDBrush(Guid driverUID)
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

		public Brush GetDynamicSKDBrush(FiresecAPI.SKDDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetSKDBrush(device) : GetDynamicSKDBrush(device.DriverUID, device.State);
		}
		private Brush GetDynamicSKDBrush(Guid guid, SKDDeviceState deviceState)
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
			return brush ?? PictureCacheSource.EmptyBrush;
		}

		private FrameworkElement GetDefaultSKDPicture(SKDLibraryDevice device)
		{
			var state = device.States.FirstOrDefault(x => x.Code == null && x.StateClass == XStateClass.No);
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : PictureCacheSource.EmptyPicture;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FiresecClient;
using XFiresecAPI;

namespace DeviceControls.XDevice
{
	public class XDevicePicture
	{
		private Dictionary<Guid, Brush> _xbrushes = new Dictionary<Guid, Brush>();
		private Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>> _dynamicXBrushes = new Dictionary<Guid, Dictionary<XStateClass, Dictionary<string, Brush>>>();

		internal XDevicePicture()
		{
		}

		public void LoadXCache()
		{
			_xbrushes.Clear();
			RegisterXBrush(null);
			XManager.DeviceLibraryConfiguration.XDevices.ForEach(item => RegisterXBrush(item));
		}
		public void LoadXDynamicCache()
		{
			_dynamicXBrushes.Clear();
			_dynamicXBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Dictionary<string, Brush>>());
			_dynamicXBrushes[Guid.Empty].Add(XStateClass.No, new Dictionary<string, Brush>());
			_dynamicXBrushes[Guid.Empty][XStateClass.No].Add(string.Empty, PictureCacheSource.EmptyBrush);
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

		private void RegisterXBrush(LibraryXDevice libraryXDevice)
		{
			var frameworkElement = libraryXDevice == null ? PictureCacheSource.EmptyPicture : GetDefaultXPicture(libraryXDevice);
			var brush = new VisualBrush(frameworkElement);
			if (_xbrushes.ContainsKey(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId))
				_xbrushes[libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId] = brush;
			else
				_xbrushes.Add(libraryXDevice == null ? Guid.Empty : libraryXDevice.XDriverId, brush);
		}
		private Brush CreateDynamicXBrush(List<LibraryXFrame> frames)
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames);
			return visualBrush;
		}

		public Brush GetXBrush(XFiresecAPI.XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetXBrush(driverUID);
		}
		private Brush GetXBrush(Guid driverUID)
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

		public Brush GetDynamicXBrush(XFiresecAPI.XDevice device)
		{
			return device == null || device.DriverUID == Guid.Empty || device.State == null ? GetXBrush(device) : GetDynamicXBrush(device.DriverUID, device.State);
		}
		private Brush GetDynamicXBrush(Guid guid, XState deviceState)
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
			return brush ?? PictureCacheSource.EmptyBrush;
		}

		private FrameworkElement GetDefaultXPicture(LibraryXDevice device)
		{
			var state = device.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
			return state.XFrames.Count > 0 ? Helper.GetVisual(state.XFrames[0].Image) : PictureCacheSource.EmptyPicture;
		}
	}
}

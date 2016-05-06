using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using Infrustructure.Plans.Devices;
using StrazhAPI.GK;

namespace DeviceControls
{
	public abstract class BaseDevicePicture<TLibraryState, TLibraryFrame, TDeviceState>
		where TLibraryFrame : ILibraryFrame
		where TLibraryState : ILibraryState<TLibraryFrame>
		where TDeviceState : IDeviceState
	{
		protected Dictionary<Guid, Brush> Brushes { get; private set; }
		protected Dictionary<Guid, Dictionary<XStateClass, Brush>> DynamicBrushes { get; private set; }

		public BaseDevicePicture()
		{
			Brushes = new Dictionary<Guid, Brush>();
			DynamicBrushes = new Dictionary<Guid, Dictionary<XStateClass, Brush>>();
		}

		public virtual void LoadCache()
		{
			Brushes.Clear();
			RegisterBrush(null);
			EnumerateLibrary().ForEach(item => RegisterBrush(item));
		}
		public virtual void LoadDynamicCache()
		{
			DynamicBrushes.Clear();
			DynamicBrushes.Add(Guid.Empty, new Dictionary<XStateClass, Brush>());
			DynamicBrushes[Guid.Empty].Add(DefaultState, PictureCacheSource.EmptyBrush);
			EnumerateLibrary().ForEach(item =>
			{
				if (!DynamicBrushes.ContainsKey(item.DriverId))
					DynamicBrushes.Add(item.DriverId, new Dictionary<XStateClass, Brush>());
				item.States.ForEach(state =>
				{
					if (!DynamicBrushes[item.DriverId].ContainsKey(state.StateType))
						DynamicBrushes[item.DriverId].Add(state.StateType, PictureCacheSource.CreateDynamicBrush(state.Frames));
				});
			});
		}

		private void RegisterBrush(ILibraryDevice<TLibraryState, TLibraryFrame, XStateClass> libraryDevice)
		{
			var frameworkElement = libraryDevice == null ? PictureCacheSource.EmptyPicture : GetDefaultPicture(libraryDevice);
			var brush = new VisualBrush(frameworkElement);
			if (Brushes.ContainsKey(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId))
				Brushes[libraryDevice == null ? Guid.Empty : libraryDevice.DriverId] = brush;
			else
				Brushes.Add(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId, brush);
		}

		protected Brush GetBrush(Guid driverUID)
		{
			if (!Brushes.ContainsKey(driverUID))
			{
				var libraryDevice = EnumerateLibrary().FirstOrDefault(x => x.DriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!Brushes.ContainsKey(Guid.Empty))
						RegisterBrush(null);
					return Brushes[Guid.Empty];
				}
				else
					RegisterBrush(libraryDevice);
			}
			return Brushes[driverUID];
		}
		protected Brush GetDynamicBrush(Guid guid, TDeviceState deviceState)
		{
			Brush brush = null;
			if (DynamicBrushes.ContainsKey(guid))
			{
				if (DynamicBrushes[guid].ContainsKey(deviceState.StateClass))
					brush = DynamicBrushes[guid][deviceState.StateClass];
				else if (DynamicBrushes[guid].ContainsKey(DefaultState))
					brush = DynamicBrushes[guid][DefaultState];
			}
			return brush ?? PictureCacheSource.EmptyBrush;
		}

		protected abstract IEnumerable<ILibraryDevice<TLibraryState, TLibraryFrame, XStateClass>> EnumerateLibrary();
		protected abstract XStateClass DefaultState { get; }

		protected virtual FrameworkElement GetDefaultPicture(ILibraryDevice<TLibraryState, TLibraryFrame, XStateClass> libraryDevice)
		{
			var state = libraryDevice.States.FirstOrDefault(x => x.StateType.Equals(DefaultState));
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : PictureCacheSource.EmptyPicture;
		}
	}
}
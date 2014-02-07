using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public abstract class BaseDevicePicture<TLibraryState, TLibraryFrame, TStateType, TDeviceState>
		where TLibraryFrame : ILibraryFrame
		where TLibraryState : ILibraryState<TLibraryFrame, TStateType>
		where TDeviceState : IDeviceState<TStateType>
	{
		protected Dictionary<Guid, Brush> Brushes { get; private set; }
		protected Dictionary<Guid, Dictionary<string, Dictionary<TStateType, Dictionary<string, Brush>>>> DynamicBrushes { get; private set; }

		public BaseDevicePicture()
		{
			Brushes = new Dictionary<Guid, Brush>();
			DynamicBrushes = new Dictionary<Guid, Dictionary<string, Dictionary<TStateType, Dictionary<string, Brush>>>>();
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
			DynamicBrushes.Add(Guid.Empty, new Dictionary<string, Dictionary<TStateType, Dictionary<string, Brush>>>());
			DynamicBrushes[Guid.Empty].Add(string.Empty, new Dictionary<TStateType, Dictionary<string, Brush>>());
			DynamicBrushes[Guid.Empty][string.Empty].Add(DefaultState, new Dictionary<string, Brush>());
			DynamicBrushes[Guid.Empty][string.Empty][DefaultState].Add(string.Empty, PictureCacheSource.EmptyBrush);
			EnumerateLibrary().ForEach(item =>
			{
				if (!DynamicBrushes.ContainsKey(item.DriverId))
				{
					DynamicBrushes.Add(item.DriverId, new Dictionary<string, Dictionary<TStateType, Dictionary<string, Brush>>>());
					DynamicBrushes[item.DriverId].Add(string.Empty, new Dictionary<TStateType, Dictionary<string, Brush>>());
				}
				item.States.ForEach(state =>
				{
					if (!DynamicBrushes[item.DriverId][string.Empty].ContainsKey(state.StateType))
						DynamicBrushes[item.DriverId][string.Empty].Add(state.StateType, new Dictionary<string, Brush>());
					if (!DynamicBrushes[item.DriverId][string.Empty][state.StateType].ContainsKey(state.Code ?? string.Empty))
						DynamicBrushes[item.DriverId][string.Empty][state.StateType].Add(state.Code ?? string.Empty, PictureCacheSource.CreateDynamicBrush(state.Frames));
				});
				AddAdditionalDynamicBrushes(item);
			});
		}

		private void RegisterBrush(ILibraryDevice<TLibraryState, TLibraryFrame, TStateType> libraryDevice)
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
		protected Brush GetDynamicBrush(Guid guid, string presenterKey, TDeviceState deviceState)
		{
			Brush brush = null;
			if (DynamicBrushes.ContainsKey(guid))
			{
				if (!string.IsNullOrEmpty(presenterKey) && DynamicBrushes[guid].ContainsKey(presenterKey))
					brush = GetDynamicBrush(DynamicBrushes[guid][presenterKey], deviceState);
				if (brush == null)
					brush = GetDynamicBrush(DynamicBrushes[guid][string.Empty], deviceState);
			}
			return brush ?? PictureCacheSource.EmptyBrush;
		}

		private FrameworkElement GetDefaultPicture(ILibraryDevice<TLibraryState, TLibraryFrame, TStateType> libraryDevice)
		{
			var state = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType.Equals(DefaultState));
			return state.Frames.Count > 0 ? Helper.GetVisual(state.Frames[0].Image) : PictureCacheSource.EmptyPicture;
		}

		protected abstract IEnumerable<ILibraryDevice<TLibraryState, TLibraryFrame, TStateType>> EnumerateLibrary();
		protected abstract TStateType DefaultState { get; }

		protected virtual void AddAdditionalDynamicBrushes(ILibraryDevice<TLibraryState, TLibraryFrame, TStateType> libraryDevice)
		{
		}
		protected virtual Brush GetDynamicBrush(Dictionary<TStateType, Dictionary<string, Brush>> map, TDeviceState deviceState)
		{
			Brush brush = null;
			var brushes = map.ContainsKey(deviceState.StateType) ? map[deviceState.StateType] : null;
			brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			if (brush == null && map.ContainsKey(DefaultState))
			{
				brushes = map[DefaultState];
				brush = brushes != null && brushes.ContainsKey(string.Empty) ? brushes[string.Empty] : null;
			}
			return brush ?? PictureCacheSource.EmptyBrush;
		}
	}
}

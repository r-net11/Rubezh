using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.AutomationCallback;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhMonitor.Layout.ViewModels
{
	public class LayoutDialogViewModel : DialogViewModel
	{
		static Dictionary<LayoutDialogViewModel, string> _dialogs = new Dictionary<LayoutDialogViewModel, string>();
		public ShowDialogCallbackData Data { get; private set; }

		public static void Show(ShowDialogCallbackData data)
		{
			var windowViewModel = new LayoutDialogViewModel(data.Layout)
			{
				Data = data,
				Title = data.Title,
				AllowClose = data.AllowClose,
				AllowMaximize = data.AllowMaximize,
				Sizable = data.Sizable,
				TopMost = data.TopMost,
				Width = data.Width,
				Height = data.Height,
				MinWidth = data.MinWidth,
				MinHeight = data.MinHeight
			};
			ApplicationService.BeginInvoke(() =>
			{
				if (data.IsModalWindow)
					DialogService.ShowModalWindow(windowViewModel);
				else
					DialogService.ShowWindow(windowViewModel);

			});
			if (data.WindowID != null)
				_dialogs.Add(windowViewModel, data.WindowID);
		}

		public static void Close(CloseDialogCallbackData data)
		{
			if (data.WindowID != null)
				while (_dialogs.ContainsValue(data.WindowID))
				{
					var item = _dialogs.FirstOrDefault(x => x.Value == data.WindowID);
					ApplicationService.Invoke(() => item.Key.Close());
					_dialogs.Remove(item.Key);
				}
		}

		public LayoutDialogViewModel(Guid layoutUID)
		{
			var layout = ClientManager.LayoutsConfiguration.Layouts.FirstOrDefault(item => item.UID == layoutUID);
			LayoutContainer = new LayoutContainer(this, layout);
		}

		public LayoutContainer LayoutContainer { get; private set; }

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}
		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}
		double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}
		double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}
	}
}
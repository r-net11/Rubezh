using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.AutomationCallback;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace FireMonitor.Layout.ViewModels
{
	public class LayoutDialogViewModel : DialogViewModel
	{
		public static void Show(DialogCallbackData data)
		{
			var windowViewModel = new LayoutDialogViewModel(data.Layout)
			{
				Title = data.Title,
				AllowClose = data.AllowClose,
				AllowMaximize = data.AllowMaximize,
				Sizable = data.Sizable,
				TopMost = data.TopMost,
				Width = data.Width,
				Height = data.Height,
				MinWidth = data.MinWidth,
				MinHeight = data.MinHeight,
			};
			ApplicationService.BeginInvoke(() =>
			{
				if (data.IsModalWindow)
					DialogService.ShowModalWindow(windowViewModel);
				else
					DialogService.ShowWindow(windowViewModel);
			});
		}

		public LayoutDialogViewModel(Guid layoutUID)
		{
			var layout = FiresecManager.LayoutsConfiguration.Layouts.FirstOrDefault(item => item.UID == layoutUID);
			LayoutContainer = new LayoutContainer(this, layout);
		}

		public LayoutContainer LayoutContainer { get; private set; }

		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}
		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}
		private double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}
		private double _minHeight;
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

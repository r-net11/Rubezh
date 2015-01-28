using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Models;
using VideoModule.ViewModels;
using Vlc.DotNet.Core.Medias;

namespace VideoModule.Views
{
	public partial class LayoutMultiCameraView
	{
		private readonly _2X2GridView _2X2GridView = new _2X2GridView();
		private readonly _1X7GridView _1X7GridView = new _1X7GridView();
		private readonly _3X3GridView _3X3GridView = new _3X3GridView();
		private readonly _4X4GridView _4X4GridView = new _4X4GridView();
		private readonly _6X6GridView _6X6GridView = new _6X6GridView();

		public LayoutMultiCameraView()
		{
			InitializeComponent();
			VlcControls = new List<VlcControlViewModel>();
			InitializeCameras();
			_grid.Child = EnumToType(ClientSettings.RviMultiLayoutCameraSettings.MultiGridType);
		}

		public List<VlcControlViewModel> VlcControls { get; private set; }
		private void InitializeCameras()
		{
			InitializeUIElement(_1X7GridView);
			InitializeUIElement(_2X2GridView);
			InitializeUIElement(_3X3GridView);
			InitializeUIElement(_4X4GridView);
			InitializeUIElement(_6X6GridView);

			Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(() =>
			{
				foreach (var vlcControl in VlcControls)
				{
					try
					{
						var cameraUid = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == vlcControl.ViewName).Value;
						if (cameraUid != Guid.Empty)
						{
							var camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == cameraUid);
							if (camera != null)
							{
								//vlcControl.VlcControlView.myVlcControl.Stop();
								//vlcControl.VlcControlView.myVlcControl.Media = new LocationMedia("rtsp://admin:admin@172.16.2.23:554/cam/realmonitor?channel=1&subtype=0");
								//vlcControl.VlcControlView.myVlcControl.Play();
								vlcControl.RviRTSP = camera.RviRTSP;
								vlcControl.Start();
								return;
							}
						}
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message);
					}
				}
			}));
		}

		void InitializeUIElement(UIElement uiElement)
		{
			var controls = new List<VlcControlView>();
			GetLogicalChildCollection(uiElement, controls);
			foreach (var control in controls)
			{
				var vlcControlViewModel = new VlcControlViewModel();
				control.DataContext = vlcControlViewModel;
				{
					vlcControlViewModel.ViewName = control.Name;
					vlcControlViewModel.VlcControlView = control;
					VlcControls.Add(control.DataContext as VlcControlViewModel);
				}
			}
		}


		private void On_2x2Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _2X2GridView;
			ClientSettings.RviMultiLayoutCameraSettings.MultiGridType = MultiGridType._2x2GridView;
		}

		private void On_1x7Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _1X7GridView;
			ClientSettings.RviMultiLayoutCameraSettings.MultiGridType = MultiGridType._1x7GridView;
		}

		private void On_3x3Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _3X3GridView;
			ClientSettings.RviMultiLayoutCameraSettings.MultiGridType = MultiGridType._3x3GridView;
		}

		private void On_4x4Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _4X4GridView;
			ClientSettings.RviMultiLayoutCameraSettings.MultiGridType = MultiGridType._4x4GridView;
		}

		private void On_6x6Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _6X6GridView;
			ClientSettings.RviMultiLayoutCameraSettings.MultiGridType = MultiGridType._6x6GridView;
		}

		private void OnShowProperties(object sender, RoutedEventArgs e)
		{
			var layoutPartPropertyCameraPageViewModel = new LayoutPartPropertyCameraPageViewModel(_grid.Child);
			if (DialogService.ShowModalWindow(layoutPartPropertyCameraPageViewModel))
			{
				foreach (var propertyViewModel in layoutPartPropertyCameraPageViewModel.PropertyViewModels)
				{
					var vlcControl = VlcControls.FirstOrDefault(x => x.ViewName == propertyViewModel.CellName);
					if (vlcControl != null && propertyViewModel.SelectedCamera != null && propertyViewModel.SelectedCamera.RviRTSP != null)
						vlcControl.RviRTSP = propertyViewModel.SelectedCamera.RviRTSP;
					var cameraUid = propertyViewModel.SelectedCamera == null ? Guid.Empty : propertyViewModel.SelectedCamera.UID;
					if (ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == propertyViewModel.CellName).Value == cameraUid)
						continue;
					try
					{
						if ((propertyViewModel.SelectedCamera != null) && (propertyViewModel.SelectedCamera.Ip != null) && vlcControl != null)
						{
							Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(vlcControl.Start));
						}
						ClientSettings.RviMultiLayoutCameraSettings.Dictionary[propertyViewModel.CellName] = cameraUid;
					}
					catch (Exception ex)
					{
						Logger.Error(ex, "LayoutMultiCameraView.OnShowProperties");
					}
				}
			}
		}

		private void OnShowArchive(object sender, RoutedEventArgs e)
		{
			var archiveViewModel = new ArchiveViewModel();
			DialogService.ShowModalWindow(archiveViewModel);
			//if (archiveViewModel.StartedRecord != null)
			//    archiveViewModel.CellPlayerWrap.Stop(archiveViewModel.StartedRecord);
		}

		public static void GetLogicalChildCollection(DependencyObject parent, List<VlcControlView> logicalCollection)
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is VlcControlView)
					{
						logicalCollection.Add(child as VlcControlView);
					}
					GetLogicalChildCollection(depChild, logicalCollection);
				}
			}
		}

		public UIElement EnumToType(MultiGridType multiGridType)
		{
			switch (multiGridType)
			{
				case MultiGridType._1x7GridView:
					return _1X7GridView;
				case MultiGridType._2x2GridView:
					return _2X2GridView;
				case MultiGridType._3x3GridView:
					return _3X3GridView;
				case MultiGridType._4x4GridView:
					return _4X4GridView;
				case MultiGridType._6x6GridView:
					return _6X6GridView;
				default: return null;
			}
		}
	}
}
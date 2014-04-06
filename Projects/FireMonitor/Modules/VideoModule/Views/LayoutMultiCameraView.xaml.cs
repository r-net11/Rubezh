using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Models;
using VideoModule.ViewModels;

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
			Loaded += UI_Loaded;
		}

		private void UI_Loaded(object sender, RoutedEventArgs e)
		{
			Cameras = new List<CameraViewModel>();
			InitializeCameras();
			InitializePerimeter();
			_grid.Child = EnumToType(ClientSettings.RviMultiLayoutCameraSettings.MultiGridType);
		}

		List<CameraViewModel> Cameras;

		void InitializeCameras()
		{
			InitializeUIElement(_1X7GridView);
			InitializeUIElement(_2X2GridView);
			InitializeUIElement(_3X3GridView);
			InitializeUIElement(_4X4GridView);
			InitializeUIElement(_6X6GridView);
		}

		void InitializeUIElement(UIElement uiElement)
		{
			var controls = new List<CellPlayerWrap>();
			GetLogicalChildCollection(uiElement, controls);
			foreach (var control in controls)
			{
				var cameraUid = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == control.Name).Value;
				var cameraViewModel = new CameraViewModel(new Camera(), control);
				if (cameraUid != Guid.Empty)
				{
					var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
					if (camera != null)
					{
						cameraViewModel.Camera = camera;
					}
				}
				Cameras.Add(cameraViewModel);
			}
		}

		void InitializePerimeter()
		{
			new Thread(delegate()
			{
				foreach (var cameraViewModel in Cameras)
				{
					try
					{
						if (cameraViewModel.Address != null)
							cameraViewModel.ConnectAndStart();
					}
					catch { }
				}
			}).Start();
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
					var cameraViewModel = Cameras.FirstOrDefault(x => x.CellPlayerWrap.Name == propertyViewModel.CellName);
					cameraViewModel.Camera = propertyViewModel.SelectedCamera;
					var cameraUid = propertyViewModel.SelectedCamera == null ? Guid.Empty : propertyViewModel.SelectedCamera.UID;
					if (ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == propertyViewModel.CellName).Value == cameraUid)
						continue;
						try
						{
							cameraViewModel.StopVideo();
							if (propertyViewModel.SelectedCamera.Address != null)
							{
								new Thread(delegate()
									{
										cameraViewModel.ConnectAndStart();
									}).Start();
							}
							ClientSettings.RviMultiLayoutCameraSettings.Dictionary[propertyViewModel.CellName] = propertyViewModel.SelectedCamera.UID;
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
			DialogService.ShowWindow(archiveViewModel);
		}

		public static void GetLogicalChildCollection(DependencyObject parent, List<CellPlayerWrap> logicalCollection)
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is CellPlayerWrap)
					{
						logicalCollection.Add(child as CellPlayerWrap);
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
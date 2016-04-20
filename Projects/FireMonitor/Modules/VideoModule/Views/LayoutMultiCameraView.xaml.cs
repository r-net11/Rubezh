using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Common;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;
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

		public List<LayoutPartCameraView> LayoutPartCameraViews { get; private set; }

		public LayoutMultiCameraView()
		{
			InitializeComponent();
			InitializeCameras();
			_grid.Child = EnumToType(ClientSettings.RviMultiLayoutCameraSettings.MultiGridType);
		}


		private void InitializeCameras()
		{
			LayoutPartCameraViews = new List<LayoutPartCameraView>();
			var controls = new List<LayoutPartCameraView>();
			GetLogicalChildCollection(_1X7GridView, controls);
			LayoutPartCameraViews.AddRange(controls);
			GetLogicalChildCollection(_2X2GridView, controls);
			LayoutPartCameraViews.AddRange(controls);
			GetLogicalChildCollection(_3X3GridView, controls);
			LayoutPartCameraViews.AddRange(controls);
			GetLogicalChildCollection(_4X4GridView, controls);
			LayoutPartCameraViews.AddRange(controls);
			GetLogicalChildCollection(_6X6GridView, controls);
			LayoutPartCameraViews.AddRange(controls);
			InitializeUIElement();
		}
		void InitializeUIElement()
		{
			foreach (var layoutPartCameraView in LayoutPartCameraViews)
			{
				var cameraUid = ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == layoutPartCameraView.Name).Value;
				var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
				if (camera != null)
				{
					var layoutPartCameraViewModel = LayoutPartCameraHelper.LayoutPartCameraViewModels.FirstOrDefault(x => x.Camera == camera);
					if (layoutPartCameraViewModel != null)
						layoutPartCameraView.DataContext = layoutPartCameraViewModel;
				}
				else
				{
					layoutPartCameraView.DataContext = new LayoutPartCameraViewModel();
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
					if (propertyViewModel.SelectedCamera == null)
					{
						continue;
					}
					var layoutPartCameraViewModel = LayoutPartCameraHelper.LayoutPartCameraViewModels.FirstOrDefault(x => x.Camera == propertyViewModel.SelectedCamera);
					if (layoutPartCameraViewModel == null)
						continue;
					var layoutPartCameraView = LayoutPartCameraViews.FirstOrDefault(x => x.Name == propertyViewModel.CellName);
					if (layoutPartCameraView != null)
						layoutPartCameraView.DataContext = layoutPartCameraViewModel;
					var cameraUid = propertyViewModel.SelectedCamera == null ? Guid.Empty : propertyViewModel.SelectedCamera.UID;
					if (ClientSettings.RviMultiLayoutCameraSettings.Dictionary.FirstOrDefault(x => x.Key == propertyViewModel.CellName).Value == cameraUid)
						continue;
					ClientSettings.RviMultiLayoutCameraSettings.Dictionary[propertyViewModel.CellName] = cameraUid;
					InitializeUIElement();
				}
			}
		}
		public static void GetLogicalChildCollection(DependencyObject parent, List<LayoutPartCameraView> logicalCollection)
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is LayoutPartCameraView)
					{
						logicalCollection.Add(child as LayoutPartCameraView);
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
using System;
using System.Collections.Generic;
using System.Windows;
using Infrastructure;
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
		private bool IsFullScreen { get; set; }
		private UIElement BackUpView { get; set; }

		public LayoutMultiCameraView()
		{
			InitializeComponent();
			Loaded += UI_Loaded;
		}

		private void UI_Loaded(object sender, RoutedEventArgs e)
		{
			InitializeUIElement(_2X2GridView, ClientSettings.MultiLayoutMultiLayoutCameraSettings._2X2Collection);
			InitializeUIElement(_1X7GridView, ClientSettings.MultiLayoutMultiLayoutCameraSettings._1X7Collection);
			InitializeUIElement(_3X3GridView, ClientSettings.MultiLayoutMultiLayoutCameraSettings._3X3Collection);
			InitializeUIElement(_4X4GridView, ClientSettings.MultiLayoutMultiLayoutCameraSettings._4X4Collection);
			InitializeUIElement(_6X6GridView, ClientSettings.MultiLayoutMultiLayoutCameraSettings._6X6Collection);
			_grid.Child = _2X2GridView;
		}

		private void InitializeUIElement(UIElement uiElement, Dictionary<string, Guid> dictionary)
		{
			var controls =
				GetLogicalChildCollection<LayoutPartCameraView>(uiElement)
					.FindAll(x => !String.IsNullOrEmpty(x.Name));
			foreach (var control in controls)
			{
				control.DataContext = new LayoutPartCameraViewModel(control.Name, dictionary);
				control.MouseDoubleClick += FullScreenSize;
			}
		}

		private void On_2x2Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _2X2GridView;
			IsFullScreen = false;
		}

		private void On_1x7Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _1X7GridView;
			IsFullScreen = false;
		}

		private void On_3x3Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _3X3GridView;
			IsFullScreen = false;
		}

		private void On_4x4Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _4X4GridView;
			IsFullScreen = false;
		}

		private void On_6x6Button_Click(object sender, RoutedEventArgs e)
		{
			_grid.Child = _6X6GridView;
			IsFullScreen = false;
		}

		private List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
		{
			var logicalCollection = new List<T>();
			GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
			return logicalCollection;
		}

		private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection)
			where T : DependencyObject
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is T)
					{
						logicalCollection.Add(child as T);
					}
					GetLogicalChildCollection(depChild, logicalCollection);
				}
			}
		}

		public void FullScreenSize(object sender, RoutedEventArgs e)
		{
			if (IsFullScreen)
			{
				_grid.Child = BackUpView;
			}
			else
			{
				BackUpView = _grid.Child;
				_grid.Child = new LayoutPartCameraView { DataContext = (sender as LayoutPartCameraView).DataContext };
				(_grid.Child as LayoutPartCameraView).MouseDoubleClick += FullScreenSize;
			}
			IsFullScreen = !IsFullScreen;
		}
	}
}
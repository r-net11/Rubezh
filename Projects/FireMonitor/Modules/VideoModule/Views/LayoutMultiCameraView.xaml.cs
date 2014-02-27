using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutMultiCameraView
	{
		public LayoutMultiCameraView()
		{
			InitializeComponent();
			Loaded += UI_Loaded;
		}

		private void UI_Loaded(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _2X2GridView());
		}

		void CreateGrid(UserControl userControl)
		{
			IsFullScreen = false;
			var layoutPartCameraViewModel = new LayoutPartCameraViewModel(((LayoutMultiCameraViewModel)DataContext).CameraViewModels.FirstOrDefault().Camera);
			var controls = GetLogicalChildCollection<Border>(userControl).FindAll(x => !String.IsNullOrEmpty(x.Name));
			foreach (var border in controls)
			{
				border.Child = new LayoutPartCameraView { DataContext = layoutPartCameraViewModel };
				border.MouseLeftButtonDown += FullScreenSize;
			}
			_grid.Child = userControl;
		}

		bool IsFullScreen { get; set; }
		UIElement BackUpView { get; set; }
		public void FullScreenSize(object sender, RoutedEventArgs e)
		{
			if (IsFullScreen)
			{
				_grid.Child = BackUpView;
			}
			else
			{
				BackUpView = _grid.Child;
				_grid.Child = new LayoutPartCameraView { DataContext = ((sender as Border).Child as LayoutPartCameraView).DataContext };
				_grid.Child.MouseLeftButtonDown += FullScreenSize;
			}
			IsFullScreen = !IsFullScreen;
		}

		public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
		{
			var logicalCollection = new List<T>();
			GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
			return logicalCollection;
		}
		private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
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

		private void On_2x2Button_Click(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _2X2GridView());
		}

		private void On_1x7Button_Click(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _1X7GridView());
		}

		private void On_3x3Button_Click(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _3X3GridView());
		}

		private void On_4x4Button_Click(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _4X4GridView());
		}

		private void On_6x6Button_Click(object sender, RoutedEventArgs e)
		{
			CreateGrid(new _6X6GridView());
		}
	}
}
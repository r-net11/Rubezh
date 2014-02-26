using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infrastructure.Client.Layout.Views;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	/// <summary>
	/// Логика взаимодействия для LayoutMultiCameraView.xaml
	/// </summary>
	public partial class LayoutMultiCameraView
	{
		private bool flag = false;
		public LayoutMultiCameraView()
		{
			InitializeComponent();

		}

		void CreateGrid()
		{
			var layoutPartCameraViewModel = new LayoutPartCameraViewModel(((LayoutMultiCameraViewModel)DataContext).CameraViewModels.FirstOrDefault().Camera);
			var _3X3GridView = new _3X3GridView();
			var controls = GetLogicalChildCollection<Grid>(_3X3GridView).FindAll(x => !String.IsNullOrEmpty(x.Name));
			foreach (var grid in controls)
				grid.Children.Add(new LayoutPartCameraView { DataContext = layoutPartCameraViewModel });
			_grid.Children.Add(_3X3GridView);
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

		private void _grid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!flag)
				CreateGrid();
			flag = true;
		}
	}
}

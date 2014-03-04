using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class _2X2GridView
	{
		public _2X2GridView()
		{
			InitializeComponent();
            var controls = GetLogicalChildCollection<LayoutPartCameraView>(this).FindAll(x => !String.IsNullOrEmpty(x.Name));
		    foreach (var control in controls)
		    {
                var layoutPartCameraViewModel = new LayoutPartCameraViewModel(control.Name, MultiGridHelper._2X2Collection);
                //layoutPartCameraViewModel.Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault();
                control.DataContext = layoutPartCameraViewModel;
		    }
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
	}
}
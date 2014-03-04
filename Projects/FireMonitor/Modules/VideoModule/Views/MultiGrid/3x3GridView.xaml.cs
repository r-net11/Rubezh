using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class _3X3GridView
	{
		public _3X3GridView()
		{
			InitializeComponent();
            var controls = GetLogicalChildCollection<LayoutPartCameraView>(this).FindAll(x => !String.IsNullOrEmpty(x.Name));
            foreach (var control in controls)
            {
                var layoutPartCameraViewModel = new LayoutPartCameraViewModel(control.Name, MultiGridHelper._3X3Collection);
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
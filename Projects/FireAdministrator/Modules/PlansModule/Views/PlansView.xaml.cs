using System;
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
using DiagramDesigner;

namespace PlansModule.Views
{
    public partial class PlansView : UserControl
    {
        public PlansView()
        {
            InitializeComponent();
        }

        bool IsPointAdding = false;

        private void OnAddPolygonPoint(object sender, RoutedEventArgs e)
        {
            IsPointAdding = true;
        }

        //private void MyDesignerCanvas_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (IsPointAdding)
        //    {
        //        if (MyDesignerCanvas.SelectedItems == null)
        //            return;
        //        var selectedItem = MyDesignerCanvas.SelectedItems.FirstOrDefault();
        //        if (selectedItem == null)
        //            return;

        //        IsPointAdding = false;

        //        var item = selectedItem;
        //        var polygon = item.Content as Polygon;


        //        Point currentPoint = e.GetPosition(selectedItem);
        //        var minDistance = double.MaxValue;
        //        int minIndex = 0;
        //        for (int i = 0; i < polygon.Points.Count; i++)
        //        {
        //            var polygonPoint = polygon.Points[i];
        //            var distance = Math.Pow(currentPoint.X - polygonPoint.X, 2) + Math.Pow(currentPoint.Y - polygonPoint.Y, 2);
        //            if (distance < minDistance)
        //            {
        //                minIndex = i;
        //                minDistance = distance;
        //            }
        //        }
        //        minIndex = minIndex + 1;
        //        Point point = e.GetPosition(selectedItem);
        //        polygon.Points.Insert(minIndex, point);


        //        PolygonResizeChrome.Current.Initialize();

        //        e.Handled = true;
        //    }
        //}
    }
}

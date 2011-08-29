using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PlansModule.ViewModels
{
    public class ZonaViewModel : BaseViewModel
    {

        public ZonaViewModel(Plan plan)
        {
            Parent = plan;
        }

        public void Initialize(List<PolygonPoint> polygonPoints)
        {
            if (PolygonPoints == null) PolygonPoints = new List<PolygonPoint>();
            PolygonPoints = polygonPoints;
        }
        public void AddPolygonPoint(PolygonPoint point)
        {
            PolygonPoints.Add(point);
        }
        public Plan Parent { get; private set; }
        public List<PolygonPoint> PolygonPoints { get; set; }

 
        public string ZoneNo { get; set; }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}
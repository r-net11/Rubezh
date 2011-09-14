using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common;

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
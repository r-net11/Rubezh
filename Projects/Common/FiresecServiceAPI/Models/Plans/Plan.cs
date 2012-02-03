using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Plan
    {
        public Plan()
        {
            UID = Guid.NewGuid();
            Children = new List<Plan>();
            ElementSubPlans = new List<ElementSubPlan>();
            Caption = "Новый план";
            Width = 0;
            Height = 0;
            BackgroundColor = Colors.LightGreen;
            ClearElements();
        }

        public void ClearElements()
        {
            ElementSubPlans = new List<ElementSubPlan>();
            ElementRectangleZones = new List<ElementRectangleZone>();
            ElementPolygonZones = new List<ElementPolygonZone>();
            ElementDevices = new List<ElementDevice>();
            ElementRectangles = new List<ElementRectangle>();
            ElementEllipses = new List<ElementEllipse>();
            ElementTextBlocks = new List<ElementTextBlock>();
            ElementPolygons = new List<ElementPolygon>();
            ElementPolylines = new List<ElementPolyline>();
        }

        public Plan Parent { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public List<Plan> Children { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }


        [DataMember]
        public List<ElementSubPlan> ElementSubPlans { get; set; }

        [DataMember]
        public List<ElementRectangleZone> ElementRectangleZones { get; set; }

        [DataMember]
        public List<ElementPolygonZone> ElementPolygonZones { get; set; }

        [DataMember]
        public List<ElementDevice> ElementDevices { get; set; }

        [DataMember]
        public List<ElementRectangle> ElementRectangles { get; set; }

        [DataMember]
        public List<ElementEllipse> ElementEllipses { get; set; }

        [DataMember]
        public List<ElementTextBlock> ElementTextBlocks { get; set; }

        [DataMember]
        public List<ElementPolygon> ElementPolygons { get; set; }

        [DataMember]
        public List<ElementPolyline> ElementPolylines { get; set; }
    }
}

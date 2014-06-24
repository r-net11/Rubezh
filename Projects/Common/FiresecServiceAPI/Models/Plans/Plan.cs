using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	[KnownType(typeof(ElementRectangleTank))]
	[KnownType(typeof(ElementCamera))]
	public class Plan : IElementBackground, IElementRectangle
	{
		public Plan()
		{
			UID = Guid.NewGuid();
			Children = new List<Plan>();
			ElementSubPlans = new List<ElementSubPlan>();
			Caption = "Новый план";
			Width = 297;
			Height = 210;
			BackgroundColor = Colors.White;
			IsVectorImage = false;
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
			ElementXDevices = new List<ElementXDevice>();
			ElementRectangleXZones = new List<ElementRectangleXZone>();
			ElementPolygonXZones = new List<ElementPolygonXZone>();
			ElementRectangleXGuardZones = new List<ElementRectangleXGuardZone>();
			ElementPolygonXGuardZones = new List<ElementPolygonXGuardZone>();
			ElementRectangleXDirections = new List<ElementRectangleXDirection>();
			ElementPolygonXDirections = new List<ElementPolygonXDirection>();

			ElementSKDDevices = new List<ElementSKDDevice>();
			ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
			ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
			ElementDoors = new List<ElementDoor>();

			ElementExtensions = new List<ElementBase>();
		}

		public Plan Parent { get; set; }

		[DataMember]
		public string Caption { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Plan> Children { get; set; }

		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public double Width { get; set; }
		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
		[DataMember]
		public Guid? BackgroundImageSource { get; set; }
		[DataMember]
		public string BackgroundSourceName { get; set; }
		[DataMember]
		public bool IsVectorImage { get; set; }
	
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
		[DataMember]
		public List<ElementRectangleZone> ElementRectangleZones { get; set; }
		[DataMember]
		public List<ElementPolygonZone> ElementPolygonZones { get; set; }
		[DataMember]
		public List<ElementSubPlan> ElementSubPlans { get; set; }
		[DataMember]
		public List<ElementDevice> ElementDevices { get; set; }
		[DataMember]
		public List<ElementXDevice> ElementXDevices { get; set; }
		[DataMember]
		public List<ElementRectangleXZone> ElementRectangleXZones { get; set; }
		[DataMember]
		public List<ElementPolygonXZone> ElementPolygonXZones { get; set; }
		[DataMember]
		public List<ElementRectangleXGuardZone> ElementRectangleXGuardZones { get; set; }
		[DataMember]
		public List<ElementPolygonXGuardZone> ElementPolygonXGuardZones { get; set; }
		[DataMember]
		public List<ElementRectangleXDirection> ElementRectangleXDirections { get; set; }
		[DataMember]
		public List<ElementPolygonXDirection> ElementPolygonXDirections { get; set; }
		[DataMember]
		public List<ElementSKDDevice> ElementSKDDevices { get; set; }
		[DataMember]
		public List<ElementRectangleSKDZone> ElementRectangleSKDZones { get; set; }
		[DataMember]
		public List<ElementPolygonSKDZone> ElementPolygonSKDZones { get; set; }
		[DataMember]
		public List<ElementDoor> ElementDoors { get; set; }

		[DataMember]
		public List<ElementBase> ElementExtensions { get; set; }

		public List<ElementBase> ElementUnion
		{
			get
			{
				var union = new List<ElementBase>();
				union.AddRange(ElementDevices);
				union.AddRange(ElementXDevices);
				union.AddRange(ElementSKDDevices);
				union.AddRange(ElementDoors);
				return union;
			}
		}

		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}
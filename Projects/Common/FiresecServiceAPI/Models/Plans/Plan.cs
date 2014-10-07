using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	[KnownType(typeof(ElementRectangleTank))]
	[KnownType(typeof(ElementCamera))]
	[KnownType(typeof(ElementProcedure))]
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

			ElementGKDevices = new List<ElementGKDevice>();
			ElementRectangleGKZones = new List<ElementRectangleGKZone>();
			ElementPolygonGKZones = new List<ElementPolygonGKZone>();
			ElementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
			ElementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
			ElementRectangleGKDirections = new List<ElementRectangleGKDirection>();
			ElementPolygonGKDirections = new List<ElementPolygonGKDirection>();

			ElementSKDDevices = new List<ElementSKDDevice>();
			ElementRectangleSKDZones = new List<ElementRectangleSKDZone>();
			ElementPolygonSKDZones = new List<ElementPolygonSKDZone>();
			ElementDoors = new List<ElementDoor>();

			ElementExtensions = new List<ElementBase>();
		}

		[XmlIgnore]
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
		public List<ElementGKDevice> ElementGKDevices { get; set; }
		[DataMember]
		public List<ElementRectangleGKZone> ElementRectangleGKZones { get; set; }
		[DataMember]
		public List<ElementPolygonGKZone> ElementPolygonGKZones { get; set; }
		[DataMember]
		public List<ElementRectangleGKGuardZone> ElementRectangleGKGuardZones { get; set; }
		[DataMember]
		public List<ElementPolygonGKGuardZone> ElementPolygonGKGuardZones { get; set; }
		[DataMember]
		public List<ElementRectangleGKDirection> ElementRectangleGKDirections { get; set; }
		[DataMember]
		public List<ElementPolygonGKDirection> ElementPolygonGKDirections { get; set; }
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

		[XmlIgnore]
		public List<ElementBase> ElementUnion
		{
			get
			{
				var union = new List<ElementBase>();
				union.AddRange(ElementDevices);
				union.AddRange(ElementDoors);
				union.AddRange(ElementExtensions);
				union.AddRange(ElementPolygonSKDZones);
				union.AddRange(ElementPolygonGKDirections);
				union.AddRange(ElementPolygonGKGuardZones);
				union.AddRange(ElementPolygonGKZones);
				union.AddRange(ElementPolygonZones);
				union.AddRange(ElementRectangleSKDZones);
				union.AddRange(ElementRectangleGKDirections);
				union.AddRange(ElementRectangleGKGuardZones);
				union.AddRange(ElementRectangleGKZones);
				union.AddRange(ElementRectangleZones);
				union.AddRange(ElementSKDDevices);
				union.AddRange(ElementSubPlans);
				union.AddRange(ElementGKDevices);
				return union;
			}
		}

		[XmlIgnore]
		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}
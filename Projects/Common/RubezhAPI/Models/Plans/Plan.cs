using Common;
using Infrustructure.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	[KnownType(typeof(ElementRectangleTank))]
	[KnownType(typeof(ElementCamera))]
	[KnownType(typeof(ElementProcedure))]
	[XmlInclude(typeof(PlanFolder)), XmlInclude(typeof(PlanFolder)), XmlInclude(typeof(ElementRectangleTank)), XmlInclude(typeof(ElementCamera)), XmlInclude(typeof(ElementProcedure))]
	public class Plan : IElementBackground, IElementRectangle
	{
		public Plan()
		{
			UID = Guid.NewGuid();
			Children = new List<Plan>();
			ElementSubPlans = new List<ElementRectangleSubPlan>();
			ElementPolygonSubPlans = new List<ElementPolygonSubPlan>();
			Caption = "Новый план";
			Width = 297;
			Height = 210;
			BackgroundColor = Colors.White;
			ImageType = ResourceType.Image;
			ClearElements();
		}

		public void ClearElements()
		{
			ElementSubPlans = new List<ElementRectangleSubPlan>();
			ElementPolygonSubPlans = new List<ElementPolygonSubPlan>();
			ElementRectangles = new List<ElementRectangle>();
			ElementEllipses = new List<ElementEllipse>();
			ElementTextBlocks = new List<ElementTextBlock>();
			ElementTextBoxes = new List<ElementTextBox>();
			ElementPolygons = new List<ElementPolygon>();
			ElementPolylines = new List<ElementPolyline>();

			ElementGKDevices = new List<ElementGKDevice>();
			ElementRectangleGKZones = new List<ElementRectangleGKZone>();
			ElementPolygonGKZones = new List<ElementPolygonGKZone>();
			ElementRectangleGKGuardZones = new List<ElementRectangleGKGuardZone>();
			ElementPolygonGKGuardZones = new List<ElementPolygonGKGuardZone>();
			ElementRectangleGKSKDZones = new List<ElementRectangleGKSKDZone>();
			ElementPolygonGKSKDZones = new List<ElementPolygonGKSKDZone>();
			ElementRectangleGKDelays = new List<ElementRectangleGKDelay>();
			ElementPolygonGKDelays = new List<ElementPolygonGKDelay>();
			ElementRectangleGKDirections = new List<ElementRectangleGKDirection>();
			ElementPolygonGKDirections = new List<ElementPolygonGKDirection>();
			ElementRectangleGKMPTs = new List<ElementRectangleGKMPT>();
			ElementPolygonGKMPTs = new List<ElementPolygonGKMPT>();
			ElementGKDoors = new List<ElementGKDoor>();

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
		public Guid? BackgroundSVGImageSource { get; set; }
		[DataMember]
		public ResourceType ImageType { get; set; }

		[DataMember]
		public List<ElementRectangle> ElementRectangles { get; set; }
		[DataMember]
		public List<ElementEllipse> ElementEllipses { get; set; }
		[DataMember]
		public List<ElementTextBlock> ElementTextBlocks { get; set; }
		[DataMember]
		public List<ElementTextBox> ElementTextBoxes { get; set; }
		[DataMember]
		public List<ElementPolygon> ElementPolygons { get; set; }
		[DataMember]
		public List<ElementPolyline> ElementPolylines { get; set; }
		[DataMember]
		public List<ElementRectangleSubPlan> ElementSubPlans { get; set; }
		[DataMember()]
		public List<ElementPolygonSubPlan> ElementPolygonSubPlans { get; set; }
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
		[DataMember()]
		public List<ElementRectangleGKDelay> ElementRectangleGKDelays { get; set; }
		[DataMember()]
		public List<ElementPolygonGKDelay> ElementPolygonGKDelays { get; set; }
		[DataMember]
		public List<ElementRectangleGKDirection> ElementRectangleGKDirections { get; set; }
		[DataMember]
		public List<ElementPolygonGKDirection> ElementPolygonGKDirections { get; set; }
		[DataMember]
		public List<ElementRectangleGKMPT> ElementRectangleGKMPTs { get; set; }
		[DataMember]
		public List<ElementPolygonGKMPT> ElementPolygonGKMPTs { get; set; }
		[DataMember]
		public List<ElementRectangleGKSKDZone> ElementRectangleGKSKDZones { get; set; }
		[DataMember]
		public List<ElementPolygonGKSKDZone> ElementPolygonGKSKDZones { get; set; }
		[DataMember]
		public List<ElementGKDoor> ElementGKDoors { get; set; }

		[DataMember]
		public List<ElementBase> ElementExtensions { get; set; }

		[XmlIgnore]
		public List<ElementBase> ElementUnion
		{
			get
			{
				var union = new List<ElementBase>();
				union.AddRange(ElementRectangleGKSKDZones);
				union.AddRange(ElementPolygonGKSKDZones);
				union.AddRange(ElementGKDoors);
				union.AddRange(ElementPolygonGKDirections);
				union.AddRange(ElementPolygonGKMPTs);
				union.AddRange(ElementPolygonGKGuardZones);
				union.AddRange(ElementPolygonGKZones);
				union.AddRange(ElementRectangleGKDirections);
				union.AddRange(ElementRectangleGKMPTs);
				union.AddRange(ElementRectangleGKGuardZones);
				union.AddRange(ElementRectangleGKZones);
				union.AddRange(ElementRectangleGKDelays);
				union.AddRange(ElementPolygonGKDelays);
				union.AddRange(ElementSubPlans);
				union.AddRange(ElementPolygonSubPlans);
				union.AddRange(ElementGKDevices);
				return union;
			}
		}
		[XmlIgnore]
		public IEnumerable<ElementBase> SimpleElements
		{
			get
			{
				foreach (var element in ElementRectangles)
					yield return element;
				foreach (var element in ElementEllipses)
					yield return element;
				foreach (var element in ElementPolylines)
					yield return element;
				foreach (var element in ElementPolygons)
					yield return element;
				foreach (var element in ElementTextBlocks)
					yield return element;
				foreach (var element in ElementTextBoxes)
					yield return element;
				foreach (var element in ElementExtensions)
					yield return element;
			}
		}
		[XmlIgnore]
		public IEnumerable<ElementBase> AllElements
		{
			get
			{
				foreach (var element in ElementUnion)
					yield return element;
				foreach (var element in SimpleElements)
					yield return element;
			}
		}

		[XmlIgnore]
		public bool AllowTransparent
		{
			get { return true; }
		}
	}
}
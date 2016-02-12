#region Usings

using System;
using System.Drawing;

#endregion

namespace GKWebService.Models.Plan.PlanElement
{
	public class Shape
	{
		public Guid Id { get; set; }
		public string SubElementId { get; set; }
		public Color Border { get; set; }
		public Color BorderMouseOver { get; set; }
		public Color Fill { get; set; }
		public Color FillMouseOver { get; set; }
		public bool HasOverlay { get; set; }
		public double BorderThickness { get; set; }
		public double Height { get; set; }
		public double Width { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public string Path { get; set; }
		public string Image { get; set; }
		public string Type { get; set; }
	}
}

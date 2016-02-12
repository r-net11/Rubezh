#region Usings

using System;
using System.Drawing;

#endregion

namespace GKWebService.Models.Plan
{
	public class Shape
	{
		public Color Border { get; set; }
		public Color BorderMouseOver { get; set; }
		public double BorderThickness { get; set; }
		public Color Fill { get; set; }
		public Color FillMouseOver { get; set; }
		public double Height { get; set; }
		public string HingImage { get; set; }
		public double HingImageHeight { get; set; }
		public double HingImageWidth { get; set; }
		public ElementHint Hint { get; set; }
		public Guid Id { get; set; }

		/// <summary>
		///     Png, преобразованный в base64 string
		/// </summary>
		public string Image { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public string Type { get; set; }
		public double Width { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
	}
}

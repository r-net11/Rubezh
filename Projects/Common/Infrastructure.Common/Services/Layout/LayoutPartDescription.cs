using System;
using System.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public LayoutPartDescription()
		{
			WidthType = GridUnitType.Star;
			HeightType = GridUnitType.Star;
			Height = 1;
			Width = 1;
			MinWidth = 25;
			MinHeight = 25;
		}

		#region ILayoutPartDescription Members

		public Guid UID { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }
		public BaseViewModel Content { get; set; }

		public GridUnitType WidthType { get; set; }
		public GridUnitType HeightType { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }

		#endregion
	}
}

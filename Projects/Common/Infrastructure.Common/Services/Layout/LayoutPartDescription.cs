using System;
using System.Windows;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public Converter<ILayoutProperties, BaseLayoutPartViewModel> Factory { get; set; }

		public LayoutPartDescription()
		{
			Size = new LayoutPartSize()
			{
				PreferedSize = new Size(100, 100)
			};
		}

		#region ILayoutPartDescription Members

		public Guid UID { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }
		public LayoutPartSize Size { get; set; }
		public BaseLayoutPartViewModel CreateContent(ILayoutProperties properties)
		{
			return Factory == null ? null : Factory(properties);
		}

		#endregion
	}
}

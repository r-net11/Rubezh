using System;
using System.Windows;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public const string IconPath = "/Controls;component/Images/";
		public Converter<ILayoutProperties, BaseLayoutPartViewModel> Factory { get; set; }

		public LayoutPartDescription(Guid uid, int index, string name, string description, string iconName = null, bool allowMultiple = true, LayoutPartSize size = null)
		{
			UID = uid;
			Index = index;
			Name = name;
			Description = description;
			if (!string.IsNullOrEmpty(iconName))
				IconSource = IconPath + iconName;
			AllowMultiple = allowMultiple;
			Size = size ?? new LayoutPartSize()
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

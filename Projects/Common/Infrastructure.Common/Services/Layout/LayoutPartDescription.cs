using System;
using System.Windows;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public const string IconPath = "/Controls;component/Images/";
		public Converter<ILayoutProperties, BaseLayoutPartViewModel> Factory { get; set; }

		//public LayoutPartDescription(Guid uid, int index, string name, string description, string iconName = null, bool allowMultiple = true, LayoutPartSize size = null)
		//	: this(LayoutPartDescriptionGroup.Root, uid, index, name, description, iconName, allowMultiple, size)
		//{
		//}

		public LayoutPartDescription(LayoutPartDescriptionGroup group, Guid uid, int index, string name, string description, string iconName = null, bool allowMultiple = true, LayoutPartSize size = null)
		{
			Group = group;
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

		public Guid UID { get; private set; }
		public int Index { get; private set; }
		public string Name { get; private set; }
		public string IconSource { get; private set; }
		public string Description { get; private set; }
		public bool AllowMultiple { get; private set; }
		public LayoutPartSize Size { get; private set; }
		public LayoutPartDescriptionGroup Group { get; private set; }

		public BaseLayoutPartViewModel CreateContent(ILayoutProperties properties)
		{
			return Factory == null ? null : Factory(properties);
		}

		#endregion
	}
}
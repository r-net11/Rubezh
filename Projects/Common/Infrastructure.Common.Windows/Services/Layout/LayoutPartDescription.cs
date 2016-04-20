using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using RubezhAPI.Automation;
using RubezhAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public const string IconPath = "/Controls;component/Images/";
		public Converter<ILayoutProperties, BaseLayoutPartViewModel> Factory { get; set; }

		public LayoutPartDescription(LayoutPartDescriptionGroup group, Guid uid, int index, string name, string description, string iconName = null, bool allowMultiple = true, LayoutPartSize size = null, IEnumerable<LayoutPartProperty> properties = null)
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
			var list = new List<LayoutPartProperty>()
			{
				new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(string), LayoutPartPropertyName.Title),
				new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(int), LayoutPartPropertyName.Margin),
				new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(Color), LayoutPartPropertyName.BackgroundColor),
				new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(Color), LayoutPartPropertyName.BorderColor),
				new LayoutPartProperty(LayoutPartPropertyAccess.GetOrSet, typeof(int), LayoutPartPropertyName.BorderThickness),
			};
			if (properties != null)
				list.AddRange(properties);
			Properties = list;
		}
		public LayoutPartDescription(LayoutPartDescriptionGroup group, Guid uid, int index, string name, string description, string iconName = null, params LayoutPartProperty[] properties)
			: this(group, uid, index, name, description, iconName, true, null, properties)
		{
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
		public IEnumerable<LayoutPartProperty> Properties { get; private set; }

		public BaseLayoutPartViewModel CreateContent(ILayoutProperties properties)
		{
			return Factory == null ? null : Factory(properties);
		}

		#endregion
	}
}
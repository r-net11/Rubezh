using System;
using System.Collections.Generic;
using RubezhAPI.Models.Layouts;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public interface ILayoutPartDescription
	{
		int Index { get; }
		Guid UID { get; }
		string Name { get; }
		string IconSource { get; }
		string Description { get; }
		LayoutPartDescriptionGroup Group { get; }
		bool AllowMultiple { get; }
		LayoutPartSize Size { get; }
		BaseLayoutPartViewModel CreateContent(ILayoutProperties properties);
		IEnumerable<LayoutPartProperty> Properties { get; }
	}
}
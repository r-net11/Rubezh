using StrazhAPI.Models.Layouts;
using System;
using System.Collections.Generic;

namespace Infrastructure.Common.Services.Layout
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
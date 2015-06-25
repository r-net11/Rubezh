﻿using System;
using System.Collections.Generic;
using FiresecAPI.Models.Layouts;

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
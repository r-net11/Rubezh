using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartDescription
	{
		int Index { get; }
		Guid UID { get; }
		string Name { get; }
		string IconSource { get; }
		string Description { get; }
		bool AllowMultiple { get; }
		BaseLayoutPartViewModel Content { get; }
		Size PreferedSize { get; }
	}
}

using System;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartDescription
	{
		int Index { get; }
		Guid UID { get; }
		string Name { get; }
		string ImageSource { get; }
		string Description { get; }
		bool AllowMultiple { get; }
	}
}

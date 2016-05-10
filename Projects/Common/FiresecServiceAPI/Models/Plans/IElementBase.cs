using System;

namespace StrazhAPI.Plans.Elements
{
	public interface IElementBase
	{
		Guid UID { get; set; }

		string PresentationName { get; set; }
	}
}
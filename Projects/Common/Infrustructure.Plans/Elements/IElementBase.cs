using System;

namespace Infrustructure.Plans.Elements
{
	public interface IElementBase
	{
		Guid UID { get; set; }
		string PresentationName { get; set; }
	}
}
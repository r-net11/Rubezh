using System;

namespace Infrustructure.Plans.Interfaces
{
	public interface IElementReference
	{
		Guid UID { get; set; }
		Guid ItemUID { get; set; }
	}
}

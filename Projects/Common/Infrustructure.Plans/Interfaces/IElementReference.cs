using System;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Interfaces
{
	public interface IElementReference : IElementBase
	{
		Guid ItemUID { get; set; }
	}
}
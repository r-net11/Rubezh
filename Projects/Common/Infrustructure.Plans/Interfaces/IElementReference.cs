using Infrustructure.Plans.Elements;
using System;

namespace Infrustructure.Plans.Interfaces
{
	public interface IElementReference : IElementBase
	{
		Guid ItemUID { get; set; }
	}
}
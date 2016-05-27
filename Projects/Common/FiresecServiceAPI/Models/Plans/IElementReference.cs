using StrazhAPI.Plans.Elements;
using System;

namespace StrazhAPI.Plans.Interfaces
{
	public interface IElementReference : IElementBase
	{
		Guid ItemUID { get; set; }
	}
}
using RubezhAPI.Plans.Elements;
using System;

namespace RubezhAPI.Plans.Interfaces
{
	public interface IElementReference : IElementBase
	{
		Guid ItemUID { get; set; }
	}
}
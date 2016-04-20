
using System;

namespace RubezhAPI.Plans.Interfaces
{
	public interface IMultipleVizualization
	{
		bool AllowMultipleVizualization { get; set; }
		Guid ItemUID { get; set; }
	}
}
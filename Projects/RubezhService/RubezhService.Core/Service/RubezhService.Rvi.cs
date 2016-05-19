using RubezhAPI;
using System.Collections.Generic;

namespace RubezhService.Service
{
	public partial class RubezhService
	{
		public List<RviState> GetRviStates()
		{
			return RviProcessor.GetRviStates();
		}
	}
}
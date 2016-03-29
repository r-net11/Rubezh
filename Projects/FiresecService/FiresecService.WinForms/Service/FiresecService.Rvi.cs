using RubezhAPI;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<RviState> GetRviStates()
		{
			return RviProcessor.GetRviStates();
		}
	}
}
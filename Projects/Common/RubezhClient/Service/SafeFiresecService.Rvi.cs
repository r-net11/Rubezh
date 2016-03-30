using RubezhAPI;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public List<RviState> GetRviStates()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetRviStates(FiresecServiceFactory.UID);
			}, "GetRviStates");
		}
	}
}
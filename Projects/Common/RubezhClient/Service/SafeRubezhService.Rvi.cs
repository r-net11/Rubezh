using RubezhAPI;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeRubezhService
	{
		public List<RviState> GetRviStates()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetRviStates(RubezhServiceFactory.UID);
			}, "GetRviStates");
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using StrazhAPI;
using StrazhAPI.Integration.OPC;

namespace FiresecClient
{
	public partial class SafeFiresecService
    {
		public OperationResult<bool> PingOPCServer()
		{
			return SafeContext.Execute(() => FiresecService.PingOPCServer());
		}

		public OperationResult<List<OPCZone>> GetOPCZones()
		{
			return SafeContext.Execute(() => FiresecService.GetOPCZones());
		}
    }
}

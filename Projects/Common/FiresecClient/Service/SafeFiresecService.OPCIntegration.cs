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

		public OperationResult<IEnumerable<OPCZone>> GetOPCZones()
		{
			return SafeContext.Execute(() => FiresecService.GetOPCZones());
		}

		public OperationResult SetGuard(int no)
		{
			return SafeContext.Execute(() => FiresecService.SetGuard(no));
		}

		public OperationResult UnsetGuard(int no)
		{
			return SafeContext.Execute(() => FiresecService.UnsetGuard(no));
		}

		public OperationResult<IEnumerable<OPCZone>> GetGuardZones()
		{
			return SafeContext.Execute(() => FiresecService.GetGuardZones());
		}
    }
}

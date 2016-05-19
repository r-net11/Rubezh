using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using StrazhAPI;

namespace FiresecClient
{
	public partial class SafeFiresecService
    {
		public OperationResult<bool> PingOPCServer()
		{
			return SafeContext.Execute(() => FiresecService.PingOPCServer());
		}
    }
}

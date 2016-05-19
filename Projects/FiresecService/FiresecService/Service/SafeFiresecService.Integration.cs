using Common;
using StrazhAPI;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> PingOPCServer()
		{
			return SafeContext.Execute(() => FiresecService.PingOPCServer());
		}
	}
}

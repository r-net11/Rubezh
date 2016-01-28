using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI;
using RubezhAPI.Automation;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<OpcDaServer[]> GetOpcDaServers()
		{
			return new OperationResult<OpcDaServer[]>();
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(OpcDaServer server)
		{
			return new OperationResult<OpcServerStatus> ();
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			return null;
		}

		public OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(OpcDaServer server)
		{
			return new OperationResult<OpcDaTagValue[]>();
		}

		public OperationResult<bool> WriteOpcDaTag(Guid tagId, object value)
		{
			return null;
		}
	}
}
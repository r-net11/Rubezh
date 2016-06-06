using System.Collections.Generic;
using System.ServiceModel;
using StrazhAPI.Integration.OPC;

namespace StrazhAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecServiceOPCIntegration
	{
		[OperationContract]
		OperationResult<bool> PingOPCServer();

		[OperationContract]
		OperationResult<IEnumerable<OPCZone>> GetOPCZones();

		[OperationContract]
		OperationResult SetGuard(int no);

		[OperationContract]
		OperationResult UnsetGuard(int no);

		[OperationContract]
		OperationResult<IEnumerable<OPCZone>> GetGuardZones();
	}
}

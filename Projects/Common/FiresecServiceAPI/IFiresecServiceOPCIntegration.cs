using System.ServiceModel;

namespace StrazhAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecServiceOPCIntegration
	{
		[OperationContract]
		OperationResult<bool> PingOPCServer();
	}
}

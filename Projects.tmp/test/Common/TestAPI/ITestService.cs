using System.ServiceModel;

namespace TestAPI
{
	[ServiceContract]
	public interface ITestService
	{
		[OperationContract]
		void Void(int clientId);
		[OperationContract(IsOneWay = true)]
		void VoidOneWay(int clientId);
		[OperationContract]
		int RandomInt(int clientId, int delay);
		[OperationContract]
		OperationResult<bool> OperationResult(int clientId);
	}
}

using System.ServiceModel;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IImitatorService
	{
		[OperationContract]
		OperationResult<string> TestImitator();
	}
}

using System.ServiceModel;

namespace FiresecAPI
{
	public interface IFiresecCallback
	{
		[OperationContract(IsOneWay = true)]
		void Ping();
	}
}
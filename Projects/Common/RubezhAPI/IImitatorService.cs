using System;
using System.ServiceModel;
using RubezhAPI.GK;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IImitatorService
	{
		[OperationContract]
		OperationResult<string> TestImitator();

		[OperationContract]
		OperationResult<bool> ConrtolGKBase(Guid uid, GKStateBit command, bool isPim);

		[OperationContract]
		OperationResult<bool> EnterCard(Guid uid, uint cardNo, GKCodeReaderEnterType enterType);
	}
}

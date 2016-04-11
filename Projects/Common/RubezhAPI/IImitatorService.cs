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
		OperationResult<bool> ConrtolGKBase(Guid uid, GKStateBit command);
	}
}

using System;
using RubezhAPI;
using RubezhAPI.GK;

namespace RubezhClient
{
	public class ImitatorService : IImitatorService
	{
		readonly ImitatorServiceFactory ImitatorServiceFactory;
		
		public ImitatorService()
		{
			ImitatorServiceFactory = new ImitatorServiceFactory();
		}

		public OperationResult<string> TestImitator()
		{
			var imitatorService = ImitatorServiceFactory.Create();
			return imitatorService.TestImitator();
		}

		public OperationResult<bool> ConrtolGKBase(Guid uid, GKStateBit command)
		{
			var imitatorService = ImitatorServiceFactory.Create();
			return imitatorService.ConrtolGKBase(uid, command);
		}

		public OperationResult<bool> EnterCard(Guid uid, uint cardNo, GKCodeReaderEnterType enterType)
		{
			var imitatorService = ImitatorServiceFactory.Create();
			return imitatorService.EnterCard(uid, cardNo, enterType);
		}
	}
}

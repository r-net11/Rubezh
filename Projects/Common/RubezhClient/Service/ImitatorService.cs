using RubezhAPI;

namespace RubezhClient
{
	public class ImitatorService : IImitatorService
	{
		readonly ImitatorServiceFactory ImitatorServiceFactory;
		
		public ImitatorService(string serverAddress)
		{
			ImitatorServiceFactory = new ImitatorServiceFactory(serverAddress);
		}

		public OperationResult<string> TestImitator()
		{
			var imitatorService = ImitatorServiceFactory.Create();
			return imitatorService.TestImitator();
		}
	}
}

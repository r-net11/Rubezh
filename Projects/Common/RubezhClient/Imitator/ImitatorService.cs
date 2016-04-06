using RubezhAPI;

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
	}
}

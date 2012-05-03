using Firesec;
using FiresecAPI;

namespace FiresecService.Service
{
	public static class FiresecOperationExtentions
	{
		public static OperationResult<T> ToOperationResult<T>(this FiresecOperationResult<T> firesecOperationResult)
		{
			var operationResult = new OperationResult<T>()
			{
				Result = firesecOperationResult.Result,
				HasError = firesecOperationResult.HasError,
				Error = firesecOperationResult.ErrorString
			};
			return operationResult;
		}
	}
}
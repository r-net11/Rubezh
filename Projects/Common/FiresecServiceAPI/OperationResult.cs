using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	[Serializable]
	public class OperationResult<T>
	{
		public OperationResult()
		{
			HasError = false;
		}

		public OperationResult(string error)
		{
			HasError = true;
			Error = error;
		}

		[DataMember]
		public T Result { get; set; }

		[DataMember]
		public bool HasError { get; set; }

		[DataMember]
		public string Error { get; set; }

		public static OperationResult<T> FromError(string error)
		{
			var operationResult = new OperationResult<T>();
			operationResult.Error = error;
			operationResult.HasError = true;
			return operationResult;
		}

		public static OperationResult<T> FromError(List<string> errors)
		{
			var operationResult = new OperationResult<T>();
			//operationResult.Error = error;
			operationResult.HasError = true;
			return operationResult;
		}
	}

	[DataContract]
	[Serializable]
	public class OperationResult
	{
		public OperationResult()
		{
			HasError = false;
		}

		public OperationResult(string error)
		{
			HasError = true;
			Error = error;
		}

		[DataMember]
		public bool HasError { get; set; }

		[DataMember]
		public string Error { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI
{
	[DataContract]
	[Serializable]
	public class OperationResult<T>
	{
		public OperationResult()
		{
			Errors = new List<string>();
		}

		public OperationResult(T result)
		{
			Errors = new List<string>();
			Result = result;
		}

		[DataMember]
		public T Result { get; set; }

		public bool HasError
		{
			get { return Errors.Count > 0; }
		}

		public string Error
		{
			get { return String.Join("\n", Errors); }
		}

		[DataMember]
		public List<string> Errors { get; set; }

		public static OperationResult<T> FromError(string error, T result = default(T))
		{
			var operationResult = new OperationResult<T>();
			if (!string.IsNullOrEmpty(error))
				operationResult.Errors.Add(error);
			operationResult.Result = result;
			return operationResult;
		}

		public static OperationResult<T> FromError(List<string> errors, T result = default(T))
		{
			var operationResult = new OperationResult<T>();
			if (errors != null)
			{
				foreach (var error in errors)
				{
					if (!string.IsNullOrEmpty(error))
						operationResult.Errors.Add(error);
				}
			}
			operationResult.Result = result;
			return operationResult;
		}

		[DataMember]
		public bool IsCanceled { get; set; }

		public static OperationResult<T> FromCancel(string error, T result = default(T))
		{
			var operationResult = FromError(error, result);
			operationResult.IsCanceled = true;
			return operationResult;
		}

		public static OperationResult<T> FromCancel(List<string> errors, T result = default(T))
		{
			var operationResult = FromError(errors, result);
			operationResult.IsCanceled = true;
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

		public OperationResult(List<string> errors)
		{
			HasError = true;
			if (errors != null && errors.Count > 0)
				Error = String.Join("\n", errors);
		}

		[DataMember]
		public bool HasError { get; set; }

		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public bool IsCanceled { get; set; }
	}
}
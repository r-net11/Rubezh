using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestAPI
{
	/// <summary>
	/// Результат операции
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract]
	[Serializable]
	public class OperationResult<T>
	{
		public OperationResult()
		{
			Errors = new List<string>();
		}

		public OperationResult(T result)
			: this()
		{
			Result = result;
		}

		[DataMember]
		public T Result { get; set; }

		/// <summary>
		/// Признак того, что есть ошибка
		/// </summary>
		public bool HasError
		{
			get { return Errors.Count > 0; }
		}

		/// <summary>
		/// Строка с ошибками
		/// </summary>
		public string Error
		{
			get { return String.Join("\n", Errors); }
		}

		/// <summary>
		/// Список строк с ошибками
		/// </summary>
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
	}
}
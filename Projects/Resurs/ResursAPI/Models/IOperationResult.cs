using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.Models
{
	public interface IOperationResult
	{
		#region Fields And Properties

		/// <summary>
		/// Успешность выполнения операции
		/// </summary>
		TransactionError Result { get; }

		/// <summary>
		/// Прочитанное или записанное значение параметра
		/// в удалённое устройтсво
		/// </summary>
		ValueType Value { get; }

		#endregion
	}
}

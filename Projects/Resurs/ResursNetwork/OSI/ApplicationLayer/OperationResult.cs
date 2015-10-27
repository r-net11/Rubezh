using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;

namespace ResursNetwork.OSI.ApplicationLayer
{
	/// <summary>
	/// Результат выполения методов ReadParameter, WriteParameter
	/// </summary>
	public class OperationResult
	{
		#region Fields And Properties

		/// <summary>
		/// Успешность выполнения операции
		/// </summary>
		public TransactionError Result { get; set; }

		/// <summary>
		/// Прочитанное или записанное значение параметра
		/// в удалённое устройтсво
		/// </summary>
		public ValueType Value { get; set; }

		#endregion
	}
}

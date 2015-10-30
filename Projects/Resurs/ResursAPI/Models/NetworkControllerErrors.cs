using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.Models
{
	/// <summary>
	/// Хранит флаги ошибок контроллера сети
	/// </summary>
	public struct NetworkControllerErrors
	{
		#region Fields And Properties

		public bool HasErrors
		{
			get { return PortError ? true : false; }
		}

		/// <summary>
		/// Ошибка порта
		/// </summary>
		public bool PortError { get; set; }

		#endregion

		#region Methods
		public void Reset()
		{
			PortError = false;
		}

		public static bool operator ==(NetworkControllerErrors str1, NetworkControllerErrors str2)
		{
			if (str1.PortError == str2.PortError) 
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool operator !=(NetworkControllerErrors str1, NetworkControllerErrors str2)
		{
			return (str1 == str2) ? false : true;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion
	}
}

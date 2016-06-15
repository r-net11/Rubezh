using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	/// <summary>
	/// Тип временного промежутка
	/// </summary>
	[DataContract]
	public enum DayIntervalPartType
	{
		/// <summary>
		/// Рабочий
		/// </summary>
		[EnumMember]
		Work,

		/// <summary>
		/// Перерыв
		/// </summary>
		[EnumMember]
		Break
	}
}
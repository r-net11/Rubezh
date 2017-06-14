using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	/// <summary>
	/// Аргументы вызова процедуры остановки выполнения тревожного правила
	/// </summary>
    [DataContract]
	public class RviAlarmStopArguments
	{
        /// <summary>
        /// Конструктор
        /// </summary>
        public RviAlarmStopArguments()
		{
			NameArgument = new Argument {ExplicitValue = {StringValue = string.Empty}};
		}

		/// <summary>
		/// Название тревожного правила
		/// </summary>
        [DataMember]
		public Argument NameArgument { get; set; }
	}
}
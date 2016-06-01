using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDLocksPassword
	{
		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool IsAppliedToLock1 { get; set; }

		[DataMember]
		public bool IsAppliedToLock2 { get; set; }

		[DataMember]
		public bool IsAppliedToLock3 { get; set; }

		[DataMember]
		public bool IsAppliedToLock4 { get; set; }

		/// <summary>
		/// Определяет количество замков, на которые распространяется пароль
		/// </summary>
		[XmlIgnore]
		public int AppliedLocksCount
		{
			get
			{
				var result = 0;
				result += IsAppliedToLock1 ? 1 : 0;
				result += IsAppliedToLock2 ? 1 : 0;
				result += IsAppliedToLock3 ? 1 : 0;
				result += IsAppliedToLock4 ? 1 : 0;
				return result;
			}
		}
	}
}

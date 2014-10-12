using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Настройки кодонаборника в охранной зоне
	/// </summary>
	[DataContract]
	public class GKCodeReaderSettings
	{
		public GKCodeReaderSettings()
		{
			SetGuardSettings = new XCodeReaderSettingsPart();
			ResetGuardSettings = new XCodeReaderSettingsPart();
			AlarmSettings = new XCodeReaderSettingsPart();

			SetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			ResetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndTwo;
		}

		/// <summary>
		/// Настройка на постановку на охрану
		/// </summary>
		[DataMember]
		public XCodeReaderSettingsPart SetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на снятие с охраны
		/// </summary>
		[DataMember]
		public XCodeReaderSettingsPart ResetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на вызов тревоги
		/// </summary>
		[DataMember]
		public XCodeReaderSettingsPart AlarmSettings { get; set; }
	}

	/// <summary>
	/// Настройка кодонаборника
	/// </summary>
	[DataContract]
	public class XCodeReaderSettingsPart
	{
		public XCodeReaderSettingsPart()
		{
			CodeReaderEnterType = GKCodeReaderEnterType.None;
		}

		/// <summary>
		/// Метод ввода
		/// </summary>
		[DataMember]
		public GKCodeReaderEnterType CodeReaderEnterType { get; set; }

		/// <summary>
		/// Идентификатор кода
		/// </summary>
		[DataMember]
		public Guid CodeUID { get; set; }
	}
}
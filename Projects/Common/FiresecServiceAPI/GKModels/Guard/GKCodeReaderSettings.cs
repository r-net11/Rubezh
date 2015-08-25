using System;
using System.Collections.Generic;
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
			SetGuardSettings = new GKCodeReaderSettingsPart();
			ResetGuardSettings = new GKCodeReaderSettingsPart();
			ChangeGuardSettings = new GKCodeReaderSettingsPart();
			AlarmSettings = new GKCodeReaderSettingsPart();
			MPTSettings = new GKCodeReaderSettingsPart();

			SetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			ResetGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndTwo;
		}

		/// <summary>
		/// Настройка на постановку на охрану
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart SetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на снятие с охраны
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart ResetGuardSettings { get; set; }

		/// <summary>
		/// Настройка на изменение состояния
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart ChangeGuardSettings { get; set; }

		/// <summary>
		/// Настройка на вызов тревоги
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart AlarmSettings { get; set; }

		/// <summary>
		/// Настройка кодов МПТ
		/// </summary>
		[DataMember]
		public GKCodeReaderSettingsPart MPTSettings { get; set; }
	}

	/// <summary>
	/// Настройка кодонаборника
	/// </summary>
	[DataContract]
	public class GKCodeReaderSettingsPart
	{
		public GKCodeReaderSettingsPart()
		{
			CodeUIDs = new List<Guid>();
			CodeReaderEnterType = GKCodeReaderEnterType.None;
		}

		/// <summary>
		/// Метод ввода
		/// </summary>
		[DataMember]
		public GKCodeReaderEnterType CodeReaderEnterType { get; set; }

		/// <summary>
		/// Идентификаторы кодов
		/// </summary>
		[DataMember]
		public List<Guid> CodeUIDs { get; set; }

		/// <summary>
		/// Минимальный уровень на проход
		/// </summary>
		[DataMember]
		public int AccessLevel { get; set; }
	}
}
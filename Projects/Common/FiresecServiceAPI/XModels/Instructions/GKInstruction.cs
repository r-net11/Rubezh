using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Инструкция ГК
	/// </summary>
	[DataContract]
	public class GKInstruction
	{
		public GKInstruction()
		{
			UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
			Devices = new List<Guid>();
			Directions = new List<Guid>();
			Name = "";
			Text = "";
		}

		/// <summary>
		/// Идентификатор
		/// </summary>
		[DataMember]
		public Guid UID { get; set; }

		/// <summary>
		/// Название
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Тип тревоги
		/// </summary>
		[DataMember]
		public GKAlarmType AlarmType { get; set; }

		/// <summary>
		/// Тип инструкции
		/// </summary>
		[DataMember]
		public GKInstructionType InstructionType { get; set; }

		/// <summary>
		/// Идентификаторы зон
		/// </summary>
		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		/// <summary>
		/// Идентификаторы зон
		/// </summary>
		[DataMember]
		public List<Guid> Devices { get; set; }

		/// <summary>
		/// Идентификаторы направлений
		/// </summary>
		[DataMember]
		public List<Guid> Directions { get; set; }

		/// <summary>
		/// Текст инструкции
		/// </summary>
		[DataMember]
		public string Text { get; set; }

		[XmlIgnore]
		public bool HasText
		{
			get
			{
				return !String.IsNullOrWhiteSpace(Text);
			}
		}

		/// <summary>
		/// Название медиа файла
		/// </summary>
		[DataMember]
		public string MediaSource { get; set; }

		[XmlIgnore]
		public bool HasMedia
		{
			get
			{
				return File.Exists(MediaSource);
			}
		}
	}
}
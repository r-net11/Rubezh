﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Прошраммно имитируемый модуль ГК
	/// </summary>
	[DataContract]
	public class GKPim : GKBase
	{
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Pim; } }

		/// <summary>
		/// Признак автогенерации
		/// </summary>
		[DataMember]
		public bool IsAutoGenerated { get; set; }

		/// <summary>
		/// Идентификатор НС
		/// </summary>
		[DataMember]
		public Guid PumpStationUID { get; set; }

		/// <summary>
		/// Идентификатор Точки Доступа
		/// </summary>
		[DataMember]
		public Guid DoorUID { get; set; }

		public override string PresentationName
		{
			get { return Name; }
		}
	}
}
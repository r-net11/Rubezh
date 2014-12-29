using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Код ГК
	/// </summary>
	[DataContract]
	public class GKCode : GKBase
	{
		public GKCode()
		{
			Name = "Новый код";
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Code; } }

		/// <summary>
		/// Пароль
		/// </summary>
		[DataMember]
		public int Password { get; set; }
	}
}
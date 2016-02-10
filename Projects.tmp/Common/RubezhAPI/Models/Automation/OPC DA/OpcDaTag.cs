using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace RubezhAPI.Automation
{
	[DataContract]
	//[KnownType(typeof(OpcDaTagValue))]
	public class OpcDaTag: OpcDaElement
	{
		#region Constructors

		public OpcDaTag() {}


		#endregion

		#region Properties

		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public string TagId { get; set; }
		/// <summary>
		/// Тип данных значения тега
		/// </summary>
		[DataMember]
		public string TypeNameOfValue { get; set; }
		/// <summary>
		/// Периoд обновление значения тега в мсек.
		/// </summary>
		[DataMember]
		public Single ScanRate { get; set; }
		/// <summary>
		/// Значение тега
		/// </summary>
		//[DataMember]
		//public OpcDaTagValue Value { get; set; }

		public override bool IsTag
		{
			get { return true; }
		}

		public bool IsChecked { get; set; }

		#endregion
	}
}
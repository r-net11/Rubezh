using RubezhAPI.Plans.Devices;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Устройство в библиотеке устройств
	/// </summary>
	[DataContract]
	public class GKLibraryDevice : ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>
	{
		public GKLibraryDevice()
		{
			UID = Guid.NewGuid();
			States = new List<GKLibraryState>();
		}

		[XmlIgnore]
		public GKDriver Driver { get; set; }

		/// <summary>
		/// Идентификатор
		/// </summary>
		[DataMember]
		public Guid UID { get; set; }

		/// <summary>
		/// Идентификатор драйвера
		/// </summary>
		[DataMember]
		public Guid DriverUID { get; set; }

		/// <summary>
		/// Состояния библиотеки устройств ГК
		/// </summary>
		[DataMember]
		public List<GKLibraryState> States { get; set; }

		#region ILibraryDevice<XStateClass,LibraryXFrame,GKState> Members

		[XmlIgnore]
		Guid ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>.DriverId
		{
			get { return DriverUID; }
			set { DriverUID = value; }
		}

		[XmlIgnore]
		List<GKLibraryState> ILibraryDevice<GKLibraryState, GKLibraryFrame, XStateClass>.States
		{
			get { return States; }
		}

		#endregion
	}
}
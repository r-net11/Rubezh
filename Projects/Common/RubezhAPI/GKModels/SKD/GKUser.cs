using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	[DataContract]
	public class GKUser
	{
		public GKUser()
		{
			GkLevel = 0;
			GkLevelSchedule = 0;
			IsActive = true;
		}

		public GKUser(ushort gkNo, Guid deviceUID)
			: this()
		{
			GkNo = gkNo;
			DeviceUID = deviceUID;
		}


		/// <summary>
		/// Порядковый номер в ГК
		/// </summary>
		[DataMember]
		public ushort GkNo { get; set; }

		[DataMember]
		public string Fio { get; set; }

		/// <summary>
		/// Пароль, для пропусков - совпадает с номером карты
		/// </summary>
		[DataMember]
		public uint Password { get; set; }

		/// <summary>
		/// Если false - по его адресу можно писать нового пользователя
		/// </summary>
		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public GKCardType UserType { get; set; }

		[DataMember]
		public DateTime ExpirationDate { get; set; }

		[DataMember]
		public byte GkLevel { get; set; }

		[DataMember]
		public byte GkLevelSchedule { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }
	}
}
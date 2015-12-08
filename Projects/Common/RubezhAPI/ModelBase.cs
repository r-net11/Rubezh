using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Базовый класс всех моделей
	/// </summary>
	[DataContract]
	public class ModelBase : IIdentity
	{
		public ModelBase()
		{
			UID = Guid.NewGuid();
		}

		/// <summary>
		/// Идентификатор
		/// </summary>
		[DataMember]
		public Guid UID { get; set; }

		/// <summary>
		/// Номер
		/// </summary>
		[DataMember]
		public int No { get; set; }

		/// <summary>
		/// Наименование
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		[DataMember]
		public string Description { get; set; }



		/// <summary>
		/// Имя для отображения названия устройства в пользовательском интерфейсе
		/// </summary>
		[XmlIgnore]
		public virtual string PresentationName
		{
			get { return No + "." + Name; }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		public void OnUIDChanged(Guid oldUID, Guid newUID)
		{
			if (UIDChanged != null)
				UIDChanged(oldUID, newUID);
		}
		public event Action<Guid, Guid> UIDChanged;
	}
}
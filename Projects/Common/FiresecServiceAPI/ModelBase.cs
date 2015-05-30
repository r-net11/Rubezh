using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;

namespace FiresecAPI.GK
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
	}
}
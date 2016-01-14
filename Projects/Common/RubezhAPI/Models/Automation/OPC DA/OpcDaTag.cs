using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using OpcClientSdk.Da;

namespace RubezhAPI.Automation
{
	[DataContract]
	[KnownType(typeof(TsDaAccessRights))]
	//[KnownType(typeof(OpcDaTagValue))]
	public class OpcDaTag: OpcDaElement
	{
		#region Constructors

		public OpcDaTag() {}
		public OpcDaTag(TsCDaBrowseElement element)
		{
			if (element.IsItem)
			{
				ElementName = element.ItemName;
				Path = element.ItemPath;
				
				var access = GetProperty(element, TsDaProperty.ACCESSRIGHTS).Value;
				if (Enum.IsDefined(typeof(TsDaAccessRights), access))
				{
					AccessRights = (TsDaAccessRights)Enum.Parse(typeof(TsDaAccessRights), access.ToString(), true);
				}
				else
				{
					throw new InvalidCastException(string.Format(
						"Невозможно приветсти {0} к типу TsDaAccessRights", access.ToString()));
				}

				ScanRate = Convert.ToSingle(GetProperty(element, TsDaProperty.SCANRATE).Value);
				TypeNameOfValue = ((Type)GetProperty(element, TsDaProperty.DATATYPE).Value).FullName;
			}
			else
			{
				throw new InvalidCastException("Невозможно создать тег на основе элемента типа группа");
			}
		}

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
		/// Модификатор доступа к значению тега
		/// </summary>
		[DataMember]
		public TsDaAccessRights AccessRights { get; set; }
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
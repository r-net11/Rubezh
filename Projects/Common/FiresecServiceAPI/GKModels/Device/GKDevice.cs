using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecClient;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Устройство ГК
	/// </summary>
	[DataContract]
	public class GKDevice : GKBase, IPlanPresentable
	{
		public GKDevice()
		{
			Children = new List<GKDevice>();
			Properties = new List<GKProperty>();
			DeviceProperties = new List<GKProperty>();
			Logic = new GKLogic();
			PlanElementUIDs = new List<Guid>();
			IsNotUsed = false;
			AllowMultipleVizualization = false;
		}

		[XmlIgnore]
		public bool IsDisabled{ get; set; }
		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Deivce; } }

		[XmlIgnore]
		public GKDevice Parent { get; set; }
		[XmlIgnore]
		public bool HasDifferences { get; set; }
		[XmlIgnore]
		public bool HasMissingDifferences { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Идентификатор драйвера
		/// </summary>
		[DataMember]
		public Guid DriverUID { get; set; }

		/// <summary>
		/// Адрес
		/// </summary>
		[DataMember]
		public byte IntAddress { get; set; }

		[DataMember]
		public string PredefinedName { get; set; }

		/// <summary>
		/// Идентификаторы дочерних устройств
		/// </summary>
		[DataMember]
		public List<GKDevice> Children { get; set; }

		/// <summary>
		/// Свойства, настроенные в системе
		/// </summary>
		[DataMember]
		public List<GKProperty> Properties { get; set; }

		/// <summary>
		/// Свойства, настроенные в приборе
		/// </summary>
		[DataMember]
		public List<GKProperty> DeviceProperties { get; set; }

		/// <summary>
		/// Логика сработки
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Разрешить множественное размещение на плане
		/// </summary>
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[XmlIgnore]
		public string Address
		{
			get
			{
				return IntAddress.ToString();
			}
		}

		[XmlIgnore]
		public string PresentationAddress
		{
			get
			{
				return Address;
			}
		}

		[XmlIgnore]
		public string DottedAddress
		{
			get
			{
				var address = new StringBuilder();

				if (address.Length > 0 && address[address.Length - 1] == '.')
					address.Remove(address.Length - 1, 1);

				return address.ToString();
			}
		}

		[XmlIgnore]
		public string DottedPresentationAddress
		{
			get
			{
				return DottedAddress;
			}
		}

		[XmlIgnore]
		public string ShortName
		{
			get { return !string.IsNullOrEmpty(PredefinedName) ? PredefinedName : string.Empty; }
		}

		[XmlIgnore]
		public override string PresentationName
		{
			get
			{
				var result = ShortName + " " + DottedPresentationAddress;
				if (!string.IsNullOrEmpty(Description))
				{
					result += "(" + Description + ")";
				}
				return result;
			}
		}

		public void SetAddress(string address)
		{
			try
			{
				byte intAddress = byte.Parse(address);
				IntAddress = intAddress;
			}
			catch { }
		}

		[XmlIgnore]
		public bool CanEditAddress
		{
			get
			{
				return Parent == null;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllParents
		{
			get
			{
				if (Parent == null)
					return new List<GKDevice>();

				List<GKDevice> allParents = Parent.AllParents;
				allParents.Add(Parent);
				return allParents;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllChildren
		{
			get
			{
				var allChildren = new List<GKDevice>();
				foreach (var child in Children)
				{
					allChildren.Add(child);
					allChildren.AddRange(child.AllChildren);
				}
				return allChildren;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllChildrenAndSelf
		{
			get
			{
				var allChildren = new List<GKDevice>();
				allChildren.Add(this);
				allChildren.AddRange(AllChildren);
				return allChildren;
			}
		}

		[XmlIgnore]
		public bool CanBeNotUsed
		{
			get { return (Parent != null); }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}
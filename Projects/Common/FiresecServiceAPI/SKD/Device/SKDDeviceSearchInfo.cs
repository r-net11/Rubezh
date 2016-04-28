using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Localization;
using System.Resources;

namespace FiresecAPI.SKD.Device
{
	[DataContract]
	public class SKDDeviceSearchInfo
	{
		[DataMember]
		public SKDDeviceType DeviceType { get; set; }

		[DataMember]
		public string IpAddress { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Submask { get; set; }

		[DataMember]
		public string Gateway { get; set; }

		[DataMember]
		public string Mac { get; set; }
	}

	public enum SKDDeviceType
    {
        //[SKDDeviceTypeLabel("Не известно")]
        [LocalizedDeviceTypeLabel(typeof(Resources.Language.SKD.Device.SKDDeviceSearchInfo), "Unknown")]
        Unknown = 0,
        #region <Контроллер Dahua>
        //[SKDDeviceTypeLabel("Однодверный контроллер", "SR-NC101")]
        [LocalizedDeviceTypeLabel(typeof(Resources.Language.SKD.Device.SKDDeviceSearchInfo), "NC101")]
        DahuaBsc1221A,
        //[SKDDeviceTypeLabel("Двухдверный контроллер", "SR-NC002")]
        [LocalizedDeviceTypeLabel(typeof(Resources.Language.SKD.Device.SKDDeviceSearchInfo), "NC002")]
        DahuaBsc1201B,
        //[SKDDeviceTypeLabel("Четырехдверный контроллер", "SR-NC004")]
        [LocalizedDeviceTypeLabel(typeof(Resources.Language.SKD.Device.SKDDeviceSearchInfo), "NC004")]
        DahuaBsc1202B
		#endregion </Контроллер Dahua>
	}

    /// <summary>
    /// Заменен классом Localized.LocalizedDeviceTypeLabel 
    /// Референсы вели в SKDDeviceTypeEnumToAttributeValueConverter 
    /// </summary>
	public class SKDDeviceTypeLabelAttribute : Attribute
	{
		public string Type { get; set; }

		public string Label { get; set; }

		/// <summary>
		/// Конструктор.
		/// Конструкция 'string label = (string)null' используется вместо 'string label = null'
		/// для обхода ошибки в компиляторе C# среды разработки VS2010
		/// См. 'http://stackoverflow.com/questions/8290853/attribute-argument-must-be-a-constant-error-when-using-an-optional-parameter-in'
		/// </summary>
		/// <param name="type">Тип</param>
		/// <param name="label">Маркировка</param>
		public SKDDeviceTypeLabelAttribute(string type, string label = (string)null)
		{
			Type = type;
			Label = label;
		}
	}
}

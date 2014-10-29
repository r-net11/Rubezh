using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlVisualArguments
	{
		public ControlVisualArguments()
		{
			Argument = new Argument();
		}

		[DataMember]
		public ControlVisualType Type { get; set; }

		[DataMember]
		public Guid Layout { get; set; }

		[DataMember]
		public Guid LayoutPart { get; set; }

		[DataMember]
		public LayoutPartPropertyName? Property { get; set; }

		[DataMember]
		public Argument Argument { get; set; }
	}

	public enum ControlVisualType
	{
		[Description("Чтение свойства")]
		Get,
		[Description("Установка свойства")]
		Set
	}

	public enum LayoutPartPropertyName
	{
		[Description("Текст")]
		Text,
	}
}
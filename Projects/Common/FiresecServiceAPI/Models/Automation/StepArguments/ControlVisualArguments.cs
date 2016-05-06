using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ControlVisualArguments : UIArguments
	{
		public ControlVisualArguments()
		{
			Argument = new Argument();
		}

		public Guid Layout
		{
			get
			{
				CheckFilter();
				return LayoutFilter.LayoutsUIDs[0];
			}
			set
			{
				CheckFilter();
				LayoutFilter.LayoutsUIDs[0] = value;
			}
		}

		[DataMember]
		public Guid LayoutPart { get; set; }

		[DataMember]
		public LayoutPartPropertyName? Property { get; set; }

		[DataMember]
		public Argument Argument { get; set; }

		[DataMember]
		public bool StoreOnServer { get; set; }

		private void CheckFilter()
		{
			if (LayoutFilter == null)
				LayoutFilter = new ProcedureLayoutCollection();
			if (LayoutFilter.LayoutsUIDs.Count == 0)
				LayoutFilter.LayoutsUIDs.Add(Guid.Empty);
		}
	}

	public enum ControlElementType
	{
		[Description("Чтение свойства")]
		Get,

		[Description("Установка свойства")]
		Set
	}

	public enum LayoutPartPropertyName
	{
		[Description("Заголовок")]
		Title,

		[Description("Отступ")]
		Margin,

		[Description("Цвет фона")]
		BackgroundColor,

		[Description("Цвет границы")]
		BorderColor,

		[Description("Ширина границы")]
		BorderThickness,

		[Description("Текст")]
		Text,
	}
}
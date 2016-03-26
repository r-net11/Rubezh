using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class ControlVisualStep : UIStep
	{
		public ControlVisualStep()
		{
			Argument = new Argument();
		}

		public Guid Layout
		{
			get
			{
				CheckFilter();
				return LayoutFilter[0];
			}
			set
			{
				CheckFilter();
				LayoutFilter[0] = value;
			}
		}

		[DataMember]
		public Guid LayoutPart { get; set; }

		[DataMember]
		public LayoutPartPropertyName? Property { get; set; }

		[DataMember]
		public Argument Argument { get; set; }

		private void CheckFilter()
		{
			if (LayoutFilter == null)
				LayoutFilter = new List<Guid>();
			if (LayoutFilter.Count == 0)
				LayoutFilter.Add(Guid.Empty);
		}
	}

	[DataContract]
	public class ControlVisualGetStep : ControlVisualStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlVisualGet; } } }
	[DataContract]
	public class ControlVisualSetStep : ControlVisualStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlVisualSet; } } }


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
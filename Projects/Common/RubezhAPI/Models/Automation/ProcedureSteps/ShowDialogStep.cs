using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowDialogStep : UIStep
	{
		public ShowDialogStep()
		{
			Width = 300;
			Height = 300;
			MinWidth = 300;
			MinHeight = 300;
			AllowClose = true;
			WindowIDArgument = new Argument();
		}

		[DataMember]
		public Guid Layout { get; set; }
		[DataMember]
		public bool IsModalWindow { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public bool AllowClose { get; set; }
		[DataMember]
		public bool AllowMaximize { get; set; }
		[DataMember]
		public bool Sizable { get; set; }
		[DataMember]
		public bool TopMost { get; set; }
		[DataMember]
		public double Width { get; set; }
		[DataMember]
		public double Height { get; set; }
		[DataMember]
		public double MinWidth { get; set; }
		[DataMember]
		public double MinHeight { get; set; }
		[DataMember]
		public bool CustomPosition { get; set; }
		[DataMember]
		public double Left { get; set; }
		[DataMember]
		public double Top { get; set; }
		[DataMember]
		public Argument WindowIDArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ShowDialog; } }
	}
}

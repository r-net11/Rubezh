using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.RightsManagement;
using Localization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ChangeListArguments
	{
		public ChangeListArguments()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public ChangeType ChangeType { get; set; }
	}

	public enum ChangeType
	{
		//[Description("Добавить в конец")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ChangeListArguments), "AddLast")]
		AddLast,

		//[Description("Удалить первый")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ChangeListArguments), "RemoveFirst")]
        RemoveFirst,

		//[Description("Удалить все")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ChangeListArguments), "RemoveAll")]
        RemoveAll
	}
}
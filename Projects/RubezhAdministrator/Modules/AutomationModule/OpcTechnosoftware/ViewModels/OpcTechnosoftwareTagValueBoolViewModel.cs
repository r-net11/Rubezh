using System;
using System.Linq;
using OpcClientSdk.Da;

namespace AutomationModule.ViewModels
{
	public class OpcTechnosoftwareTagValueBoolViewModel : OpcTechnosoftwareTagValueBaseViewModel
	{
		#region Constructors

		internal OpcTechnosoftwareTagValueBoolViewModel(TsCDaBrowseElement tag) : base(tag) { }
		
		#endregion

		#region Fields And Properties
		
		public override ValueType Value
		{
			get
			{
				return (bool)Tag.Properties
					.First(x => x.ID == TsDaProperty.VALUE).Value;
			}
			set
			{
				Tag.Properties
					.First(x => x.ID == TsDaProperty.VALUE).Value = (bool)value;
				OnPropertyChanged(() => Value);
			}
		}
		
		#endregion
	}
}

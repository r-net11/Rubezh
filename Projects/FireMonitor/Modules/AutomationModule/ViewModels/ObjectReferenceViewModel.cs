using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ObjectReferenceViewModel : BaseViewModel
	{
		ObjectReference _value;
		public ObjectReference Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged(() => Value);
				OnPropertyChanged(() => PresentationName);
			}
		}

		public ObjectReferenceViewModel(ObjectReference value)
		{
			Value = value;
		}

		public string PresentationName
		{
			get { return ExplicitValue.GetObjectString(Value); }
		}
	}
}

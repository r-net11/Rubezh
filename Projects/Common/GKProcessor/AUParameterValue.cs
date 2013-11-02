using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKProcessor
{
	public class AUParameterValue : BaseViewModel
	{
		public XDevice Device { get; set; }
		public string Name { get; set; }
		public bool IsDelay { get; set; }
		public XAUParameter DriverParameter { get; set; }

		int _value;
		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged("Value");
			}
		}

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged("StringValue");
			}
		}
	}
}
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MeasureParameterViewModel : BaseViewModel
	{
		public GKDevice Device { get; set; }
		public string Name { get; set; }
		public bool IsDelay { get; set; }
		public GKMeasureParameter DriverParameter { get; set; }

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
	}
}
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using System.Windows.Input;

namespace Infrastructure.Plans.ViewModels
{
	public class InstrumentViewModel : BaseViewModel, IInstrument
	{
		public InstrumentViewModel()
		{
			Autostart = true;
			IsActive = true;
			GroupIndex = 0;
		}

		string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		string _toolTip;
		public string ToolTip
		{
			get { return _toolTip; }
			set
			{
				_toolTip = value;
				OnPropertyChanged(() => ToolTip);
			}
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}

		int _group;
		public int GroupIndex
		{
			get { return _group; }
			set
			{
				_group = value;
				OnPropertyChanged(() => GroupIndex);
			}
		}

		public ICommand Command { get; set; }
		public InstrumentAdorner Adorner { get; set; }
		public bool Autostart { get; set; }
		public int Index { get; set; }
	}
}
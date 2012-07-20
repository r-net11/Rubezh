using System.Windows.Input;

namespace Infrustructure.Plans.Designer
{
	public interface IInstrument
	{
		string ImageSource { get; set; }
		string ToolTip { get; set; }
		ICommand Command { get; set; }
		InstrumentAdorner Adorner { get; set; }
		bool Autostart { get; set; }
	}
}

using System.Windows.Input;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrustructure.Plans.Designer
{
	public interface IInstrument
	{
		string ImageSource { get; set; }
		string ToolTip { get; set; }
		ICommand Command { get; set; }
		InstrumentAdorner Adorner { get; set; }
		bool Autostart { get; set; }
		int Index { get; set; }
	}
}

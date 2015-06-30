using System.Windows.Input;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrustructure.Plans.Designer
{
	public interface IInstrument
	{
		string ImageSource { get; }
		string ToolTip { get; }
		ICommand Command { get; }
		InstrumentAdorner Adorner { get; }
		bool Autostart { get; }
		int Index { get; }
		bool IsActive { get; }
		int GroupIndex { get; }
	}
}
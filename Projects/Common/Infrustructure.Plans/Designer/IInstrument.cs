using Infrustructure.Plans.InstrumentAdorners;
using System.Windows.Input;

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
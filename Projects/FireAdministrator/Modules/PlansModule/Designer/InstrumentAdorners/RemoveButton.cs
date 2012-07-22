using System.Windows;
using System.Windows.Controls;

namespace PlansModule.InstrumentAdorners
{
	public class RemoveButton : Control
	{
		static RemoveButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RemoveButton), new FrameworkPropertyMetadata(typeof(RemoveButton)));
		}
	}
}

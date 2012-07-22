using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

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

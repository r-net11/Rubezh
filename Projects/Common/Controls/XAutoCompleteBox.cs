using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
	public class XAutoCompleteBox : AutoCompleteBox
	{
		protected override void OnKeyDown(KeyEventArgs e)
		{
			var isOpened = IsDropDownOpen;
			base.OnKeyDown(e);
			if (!isOpened && (e.Key == Key.Enter || e.Key == Key.Return))
				e.Handled = false;
		}
	}
}

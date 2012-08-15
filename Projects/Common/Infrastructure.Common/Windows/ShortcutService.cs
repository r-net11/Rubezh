using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Common.Windows
{
	class ShortcutService
	{
		public ShortcutService()
		{
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{

		}
	}
}

using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Common.Windows
{
	public class ShortcutService
	{
		public event KeyEventHandler KeyPressed;

		internal ShortcutService()
		{
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (KeyPressed != null)
				KeyPressed(this, e);
		}
	}
}

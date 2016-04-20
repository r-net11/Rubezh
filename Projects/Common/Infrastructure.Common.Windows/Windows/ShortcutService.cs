using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Common.Windows
{
	public class ShortcutService
	{
		public event KeyEventHandler KeyPressed;
		public Dictionary<KeyGesture, RelayCommand> Shortcuts { get; private set; }

		internal ShortcutService()
		{
			Shortcuts = new Dictionary<KeyGesture, RelayCommand>();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (KeyPressed != null && !e.Handled)
				KeyPressed(this, e);
			if (!e.Handled && ApplicationService.ApplicationWindow.IsKeyboardFocusWithin)
				foreach (var keyGesture in Shortcuts.Keys)
					if (e.Key == keyGesture.Key && keyGesture.Modifiers == Keyboard.Modifiers)
					{
						RelayCommand command = Shortcuts[keyGesture];
						if (command.CanExecute(null))
							command.Execute();
					}
		}
	}
}
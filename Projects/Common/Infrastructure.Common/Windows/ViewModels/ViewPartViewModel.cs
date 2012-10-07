
using System;
using System.Collections.Generic;
using System.Windows.Input;
namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class ViewPartViewModel : BaseViewModel, IViewPartViewModel
	{
		protected Dictionary<KeyGesture, Action> Shortcuts { get; private set; }

		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			private set
			{
				_isActive = value;
				OnPropertyChanged("IsActive");
			}
		}

		public ViewPartViewModel()
		{
			Shortcuts = new Dictionary<KeyGesture, Action>();
		}

		internal void Show()
		{
			IsActive = true;
			OnShow();
			ApplicationService.Layout.ShortcutService.KeyPressed += new KeyEventHandler(ShortcutService_KeyPressed);
		}
		internal void Hide()
		{
			if (IsActive)
			{
				OnHide();
				ApplicationService.Layout.ShortcutService.KeyPressed -= new KeyEventHandler(ShortcutService_KeyPressed);
			}
			IsActive = false;
		}

		private void ShortcutService_KeyPressed(object sender, KeyEventArgs e)
		{
			if (IsActive && ApplicationService.ApplicationWindow.IsKeyboardFocusWithin)
			{
				foreach (var keyGesture in Shortcuts.Keys)
					if (e.Key == keyGesture.Key && keyGesture.Modifiers == Keyboard.Modifiers)
						Shortcuts[keyGesture]();
				KeyPressed(e);
			}
		}
		protected virtual void KeyPressed(KeyEventArgs e)
		{
		}

		#region IViewPartViewModel Members

		public virtual string Key
		{
			get { return GetType().FullName; }
		}

		public virtual void OnShow()
		{
		}
		public virtual void OnHide()
		{
		}

		public void RegisterShortcut(KeyGesture keyGesture, Action command)
		{
			Shortcuts.Add(keyGesture, command);
		}
		public void RegisterShortcut(KeyGesture keyGesture, RelayCommand command)
		{
			Shortcuts.Add(keyGesture, command.Execute);
		}
		public void RegisterShortcut<T>(KeyGesture keyGesture, RelayCommand<T> command, Func<T> getArg)
		{
			Shortcuts.Add(keyGesture, () => { command.Execute(getArg()); });
		}

		#endregion
	}
}

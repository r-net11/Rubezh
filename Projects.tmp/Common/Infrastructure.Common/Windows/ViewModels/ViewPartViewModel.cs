using System;
using System.Collections.Generic;
using System.Windows.Input;
using Infrastructure.Common.Services.Layout;
namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class ViewPartViewModel : BaseViewModel, IViewPartViewModel
	{
		protected Dictionary<KeyGesture, RelayCommand> Shortcuts { get; private set; }

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

		public bool IsRightPanelEnabled { get; set; }
		private bool _isRightPanelVisible;
		public bool IsRightPanelVisible
		{
			get { return _isRightPanelVisible; }
			set
			{
				_isRightPanelVisible = value;
				RegistrySettingsHelper.SetBool(IsRightPanelVisibleRegistryKey, IsRightPanelVisible);
			}
		}
		protected virtual bool IsRightPanelVisibleByDefault
		{
			get { return false; }
		}
		private string IsRightPanelVisibleRegistryKey
		{
			get { return "Shell.IsRightPanelVisible." + Key; }
		}

		public ViewPartViewModel()
		{
			Shortcuts = new Dictionary<KeyGesture, RelayCommand>();
			_isRightPanelVisible = RegistrySettingsHelper.GetBool(IsRightPanelVisibleRegistryKey, IsRightPanelVisibleByDefault);
		}

		internal void Show()
		{
			IsActive = true;
			OnShow();
			if (this is ILayoutPartContent)
			{
				var content = (ILayoutPartContent)this;
				if (content.Container != null)
					content.Container.Activate();
			}
			ApplicationService.Layout.ShortcutService.KeyPressed -= new KeyEventHandler(ShortcutService_KeyPressed);
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
			if (IsActive && ApplicationService.ApplicationWindow.IsKeyboardFocusWithin && !ApplicationService.Layout.IsRightPanelFocused)
			{
				foreach (var keyGesture in Shortcuts.Keys)
					if (e.Key == keyGesture.Key && keyGesture.Modifiers == Keyboard.Modifiers)
					{
						RelayCommand command = Shortcuts[keyGesture];
						if (command.CanExecute(null))
							command.Execute();
					}
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

		public void RegisterShortcut(KeyGesture keyGesture, RelayCommand command)
		{
			Shortcuts.Add(keyGesture, command);
		}
		public void RegisterShortcut<T>(KeyGesture keyGesture, RelayCommand<T> command, Func<T> getArg)
		{
			RelayCommand cmd = new RelayCommand(() => command.Execute(getArg()), () => command.CanExecute(getArg()));
			RegisterShortcut(keyGesture, cmd);
		}
		public void RegisterShortcut(KeyGesture keyGesture, Action action)
		{
			RelayCommand cmd = new RelayCommand(action);
			RegisterShortcut(keyGesture, cmd);
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
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
			
		}
		internal void Hide()
		{
			
			IsActive = false;
		}

		private void ShortcutService_KeyPressed(object sender, KeyEventArgs e)
		{
			
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

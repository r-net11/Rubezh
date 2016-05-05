using FiresecAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Windows.Input;

namespace AutomationModule.ViewModels
{
	public class AddRemoveArgumentViewModel : BaseViewModel
	{
		private readonly Action _addAction;
		private readonly Action<AddRemoveArgumentViewModel> _removeAction;

		public string ImageSource { get; private set; }

		public ArgumentViewModel Argument { get; private set; }

		public ICommand Command { get; private set; }
		private void OnAdd()
		{
			if (_addAction == null)
				return;
			_addAction();
		}
		private void OnRemove()
		{
			if (_removeAction == null)
				return;
			_removeAction(this);
		}

		public AddRemoveArgumentViewModel(Argument argument, Action updateDescriptionHandler, Action updateContentHandler, bool allowExplicitValue = true, bool allowLocalValue = true, bool allowGlobalValue = true, bool canAdd = false, Action addAction = null, Action<AddRemoveArgumentViewModel> removeAction = null)
		{
			_addAction = addAction;
			_removeAction = removeAction;
			Argument = new ArgumentViewModel(argument, updateDescriptionHandler, updateContentHandler, allowExplicitValue, allowLocalValue, allowGlobalValue);
			Command = canAdd
				? new RelayCommand(OnAdd)
				: new RelayCommand(OnRemove);
			ImageSource = canAdd
				? "Add"
				: "Delete";
		}

		public void Update(Procedure procedure, ExplicitType explicitType)
		{
			Argument.Update(procedure, explicitType);
		}
	}
}
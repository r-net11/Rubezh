using System;
using System.Windows.Input;

namespace Infrastructure.Common.Windows
{
	public class RelayCommandProvider : ICommand
	{
		private Func<ICommand> _provider;

		public static RelayCommandProvider Create(Func<ICommand> provider)
		{
			return new RelayCommandProvider(provider);
		}
		private RelayCommandProvider(Func<ICommand> provider)
		{
			_provider = provider;
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			var command = _provider();
			return command == null ? false : command.CanExecute(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			var command = _provider();
			if (command != null)
				command.Execute(parameter);
		}

		#endregion
	}
}

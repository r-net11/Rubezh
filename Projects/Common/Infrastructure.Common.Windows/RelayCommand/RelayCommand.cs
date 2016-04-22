using Common;
using Infrastructure.Common.Services;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Infrastructure.Common
{
	public delegate bool PredicateDelegate();

	public class RelayCommand : ICommand
	{
		#region Fields
		readonly Action _execute;
		readonly Predicate<object> _canExecute;
		#endregion

		#region Ctors

		[DebuggerStepThrough]
		public RelayCommand(Action execute)
			: this(execute, (Predicate<object>)null)
		{
		}

		[DebuggerStepThrough]
		public RelayCommand(Action execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public RelayCommand(Action execute, PredicateDelegate canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = (object obj) => { return canExecute(); };
		}
		#endregion

		[DebuggerStepThrough]
		public void Execute()
		{
			if (CanExecute(null))
				ForceExecute();
		}

		[DebuggerStepThrough]
		public void ForceExecute()
		{
			try
			{
				_execute();
			}
			catch (Exception e)
			{
				Logger.Error(e, "RelayCommand.ForceExecute");
				ServiceFactoryBase.MessageBoxService.ShowException(e, "При выполнении операции возникло исключение");
			}
		}

		#region ICommand Members

		[DebuggerStepThrough]
		void ICommand.Execute(object parameter)
		{
			ForceExecute();
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
			{
				try
				{
					return _canExecute(parameter);
				}
				catch (Exception e)
				{
					Logger.Error(e, "RelayCommand.CanExecute");
					return false;
				}
			}
			return true;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		#endregion
	}

	public class RelayCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;

		#endregion

		#region Ctors

		[DebuggerStepThrough]
		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		[DebuggerStepThrough]
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		[DebuggerStepThrough]
		public void Execute(T parameter)
		{
			if (CanExecute(parameter))
				ForceExecute(parameter);
		}

		[DebuggerStepThrough]
		public void ForceExecute(T parameter)
		{
			try
			{
				_execute(parameter);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RelayCommand.ForceExecute");
				ServiceFactoryBase.MessageBoxService.ShowException(e, "При выполнении операции возникло исключение");
			}
		}

		#region ICommand Members

		[DebuggerStepThrough]
		void ICommand.Execute(object parameter)
		{
			ForceExecute((T)parameter);
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
				return _canExecute((T)(parameter ?? default(T)));
			return true;
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		#endregion
	}
}
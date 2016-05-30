using System;
using System.Linq;
using AutomationModule.Models;
using AutomationModule.Properties;
using Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Windows.Controls;
using StrazhAPI;

namespace AutomationModule.ViewModels
{
	public class ScriptListViewModel : SaveCancelDialogViewModel
	{
		private bool _isBisy;
		private List<Script> _scripts;
		private Script _selectedScript;

		public bool IsBisy
		{
			get { return _isBisy; }
			set
			{
				if (_isBisy == value) return;
				_isBisy = value;
				OnPropertyChanged(() => IsBisy);
			}
		}

		public Script SelectedScript
		{
			get { return _selectedScript; }
			set
			{
				if (_selectedScript == value) return;
				_selectedScript = value;
				OnPropertyChanged(() => SelectedScript);
			}
		}

		public List<Script> Scripts
		{
			get { return _scripts; }
			set
			{
				if (_scripts == value) return;
				_scripts = value;
				OnPropertyChanged(() => Scripts);
			}
		}

		public ScriptListViewModel()
		{
			LoadScripts();
		}

		public void LoadScripts()
		{
			IsBisy = true;
			Task.Factory.StartNew(() => FiresecManager.FiresecService.GetFiresecScripts())
			.ContinueWith(t =>
			{
				if (t.IsFaulted || t.Result.HasError)
				{
					IsBisy = false;
					var ex = t.Exception;
					while (ex is AggregateException && ex.InnerException != null)
					{
						ex = (AggregateException) ex.InnerException;
						Logger.Error(ex);
					}
					MessageBoxService.ShowError(Resources.ErrorOPCScriptConnectionContent);
				}
				else
				{
					IsBisy = false;
					Scripts = t.Result.Result.Select(x => new Script(x)).ToList();
					SelectedScript = Scripts.FirstOrDefault();
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override bool CanSave()
		{
			return !IsBisy;
		}
	}
}

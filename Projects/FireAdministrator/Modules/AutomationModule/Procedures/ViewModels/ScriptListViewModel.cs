using System.Linq;
using AutomationModule.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			Task.Factory.StartNew(() =>
			{
				Scripts = FiresecManager.FiresecService.GetFiresecScripts().Result.Select(x => new Script(x)).ToList();
			})
			.ContinueWith(x =>
			{
				IsBisy = false;
				SelectedScript = Scripts.FirstOrDefault();
			});
		}

		protected override bool CanSave()
		{
			return !IsBisy;
		}
	}
}

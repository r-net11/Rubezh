using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.PassCardDesigner.Model;
using StrazhAPI.SKD;

namespace SKDModule.Employees.ViewModels.DialogWindows
{
	public class PreviewReportDialogViewModel : SaveCancelDialogViewModel
	{
		private Template _selectedTemplate;
		private Organisation _currentOrganisation;
		private ObservableCollection<Template> _templatesCollection;
		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (_isBusy == value) return;
				_isBusy = value;
				OnPropertyChanged(() => IsBusy);
			}
		}

		public ObservableCollection<Template> TemplatesCollection
		{
			get { return _templatesCollection; }
			set
			{
				_templatesCollection = value;
				OnPropertyChanged(() => TemplatesCollection);
			}
		}

		public Template SelectedTemplate
		{
			get { return _selectedTemplate; }
			set
			{
				_selectedTemplate = value;
				OnPropertyChanged(() => SelectedTemplate);
			}
		}

		public PreviewReportDialogViewModel(Organisation organisation)
		{
			if(organisation == null)
				throw new ArgumentNullException("organisation");

			_currentOrganisation = organisation;
			GetAllTemplates();
		}

		private void GetAllTemplates()
		{
			IsBusy = true;

			Task.Factory.StartNew(() => PassCardTemplateHelper.GetFullTemplateByOrganisation(_currentOrganisation.UID))
				.ContinueWith(t =>
				{
					TemplatesCollection = new ObservableCollection<Template>(t.Result.Select(x => new Template(x)));
					SelectedTemplate = TemplatesCollection.FirstOrDefault();
					IsBusy = false;
				});

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.Collections.ObjectModel;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
	public class ParameterTemplateSelectationViewModel : SaveCancelDialogViewModel
	{
		public ParameterTemplateSelectationViewModel()
		{
			Title = "Выбор шаблона";
			ParameterTemplates = new ObservableCollection<ParameterTemplate>();
			foreach (var parameterTemplate in FiresecManager.ParameterTemplates)
			{
				ParameterTemplates.Add(parameterTemplate);
			}
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
		}

		public ObservableCollection<ParameterTemplate> ParameterTemplates { get; private set; }

		ParameterTemplate _selectedParameterTemplate;
		public ParameterTemplate SelectedParameterTemplate
		{
			get { return _selectedParameterTemplate; }
			set
			{
				_selectedParameterTemplate = value;
				OnPropertyChanged("SelectedParameterTemplate");
			}
		}
	}
}
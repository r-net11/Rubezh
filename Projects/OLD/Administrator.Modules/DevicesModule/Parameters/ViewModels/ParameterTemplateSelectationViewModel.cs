using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

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
				OnPropertyChanged(() => SelectedParameterTemplate);
			}
		}
	}
}
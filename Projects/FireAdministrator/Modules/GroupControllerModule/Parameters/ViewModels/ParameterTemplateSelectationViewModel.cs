using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ParameterTemplateSelectationViewModel : SaveCancelDialogViewModel
	{
		public ParameterTemplateSelectationViewModel()
		{
			Title = "Выбор шаблона";
			ParameterTemplates = new ObservableCollection<GKParameterTemplate>();
			foreach (var parameterTemplate in GKManager.ParameterTemplates)
			{
				ParameterTemplates.Add(parameterTemplate);
			}
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
		}

		public ObservableCollection<GKParameterTemplate> ParameterTemplates { get; private set; }

		GKParameterTemplate _selectedParameterTemplate;
		public GKParameterTemplate SelectedParameterTemplate
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
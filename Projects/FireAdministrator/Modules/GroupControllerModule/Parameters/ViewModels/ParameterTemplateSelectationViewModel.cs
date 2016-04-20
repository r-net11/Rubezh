using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

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
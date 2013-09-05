using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.Collections.ObjectModel;
using FiresecAPI.XModels;

namespace GKModule.ViewModels
{
	public class ParameterTemplateSelectationViewModel : SaveCancelDialogViewModel
	{
		public ParameterTemplateSelectationViewModel()
		{
			Title = "Выбор шаблона";
			ParameterTemplates = new ObservableCollection<XParameterTemplate>();
			foreach (var parameterTemplate in XManager.ParameterTemplates)
			{
				ParameterTemplates.Add(parameterTemplate);
			}
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
		}

		public ObservableCollection<XParameterTemplate> ParameterTemplates { get; private set; }

		XParameterTemplate _selectedParameterTemplate;
		public XParameterTemplate SelectedParameterTemplate
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
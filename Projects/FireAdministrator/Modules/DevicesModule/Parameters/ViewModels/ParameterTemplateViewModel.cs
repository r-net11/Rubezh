using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
	public class ParameterTemplateViewModel : BaseViewModel
	{
		public ParameterTemplate ParameterTemplate { get; private set; }

		public ParameterTemplateViewModel(ParameterTemplate parameterTemplate)
		{
			ParameterTemplate = parameterTemplate;
			DeviceParameterTemplates = new List<DeviceParameterTemplateViewModel>();
			foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
			{
				var deviceParameterTemplateViewModel = new DeviceParameterTemplateViewModel(deviceParameterTemplate);
				DeviceParameterTemplates.Add(deviceParameterTemplateViewModel);
			}
			SelectedDeviceParameterTemplate = DeviceParameterTemplates.FirstOrDefault();
		}

		public List<DeviceParameterTemplateViewModel> DeviceParameterTemplates { get; private set; }

		DeviceParameterTemplateViewModel _selectedDeviceParameterTemplate;
		public DeviceParameterTemplateViewModel SelectedDeviceParameterTemplate
		{
			get { return _selectedDeviceParameterTemplate; }
			set
			{
				_selectedDeviceParameterTemplate = value;
				OnPropertyChanged("SelectedDeviceParameterTemplate");
			}
		}

		public string Name
		{
			get { return ParameterTemplate.Name; }
			set
			{
				ParameterTemplate.Name = value;
				OnPropertyChanged("Name");
			}
		}

		public bool IsEnabled
		{
			get { return Name != "По умолчанию"; }
		}
	}
}
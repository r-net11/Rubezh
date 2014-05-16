﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class ParameterTemplateViewModel : BaseViewModel
	{
		public XParameterTemplate ParameterTemplate { get; private set; }

		public ParameterTemplateViewModel(XParameterTemplate parameterTemplate)
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
				if (value == "По умолчанию")
					return;
				ParameterTemplate.Name = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool IsEnabled
		{
			get { return Name != "По умолчанию"; }
		}
	}
}
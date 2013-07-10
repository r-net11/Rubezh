using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure;
using System.Windows.Input;
using FiresecClient;
using FiresecAPI.Models;
using KeyboardKey = System.Windows.Input.Key;

namespace DevicesModule.ViewModels
{
	public class ParameterTemplatesViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public ParameterTemplatesViewModel()
		{
			Menu = new ParameterTemplatesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanRemove);
			RegisterShortcuts();
		}

		public void Initialize()
		{
			Invalidate();
			ParameterTemplates = new ObservableCollection<ParameterTemplateViewModel>();
			foreach (var parameterTemplate in FiresecManager.ParameterTemplates)
			{
				var parameterTemplateViewModel = new ParameterTemplateViewModel(parameterTemplate);
				ParameterTemplates.Add(parameterTemplateViewModel);
			}
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
		}

		ObservableCollection<ParameterTemplateViewModel> _parameterTemplates;
		public ObservableCollection<ParameterTemplateViewModel> ParameterTemplates
		{
			get { return _parameterTemplates; }
			set
			{
				_parameterTemplates = value;
				OnPropertyChanged("ParameterTemplates");
			}
		}

		ParameterTemplateViewModel _selectedParameterTemplate;
		public ParameterTemplateViewModel SelectedParameterTemplate
		{
			get { return _selectedParameterTemplate; }
			set
			{
				_selectedParameterTemplate = value;
				OnPropertyChanged("SelectedParameterTemplate");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var maxNo = FiresecManager.ParameterTemplates.Max(x => x.No);
			var parameterTemplate = new ParameterTemplate()
			{
				Name = "Шаблон " + (maxNo + 1).ToString(),
				No = maxNo + 1
			};
			FiresecManager.ParameterTemplates.Add(parameterTemplate);
			Invalidate();
			var parameterTemplateViewModel = new ParameterTemplateViewModel(parameterTemplate);
			ParameterTemplates.Add(parameterTemplateViewModel);
			SelectedParameterTemplate = parameterTemplateViewModel;
			ServiceFactory.SaveService.FSChanged = true;
		}

		bool CanRemove()
		{
			return SelectedParameterTemplate != null && SelectedParameterTemplate.ParameterTemplate.Name != "По умолчанию";
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			FiresecManager.ParameterTemplates.RemoveAll(x => x.UID == SelectedParameterTemplate.ParameterTemplate.UID);
			ParameterTemplates.Remove(SelectedParameterTemplate);
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
			ServiceFactory.SaveService.FSChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }

		void Invalidate()
		{
			if (FiresecManager.FiresecConfiguration.DeviceConfiguration.ParameterTemplates.Count == 0)
			{
				var parameterTemplate = new ParameterTemplate()
				{
					Name = "По умолчанию"
				};
				FiresecManager.ParameterTemplates.Add(parameterTemplate);
			}

			foreach (var parameterTemplate in FiresecManager.ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
					deviceParameterTemplate.Device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == deviceParameterTemplate.Device.DriverUID);
				}
				parameterTemplate.DeviceParameterTemplates.RemoveAll(x => x.Device.Driver == null);

				foreach (var driver in FiresecManager.Drivers)
				{
					if (driver.Properties.Any(x => x.IsAUParameter))
					{
						var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.Device.DriverUID == driver.UID);
						if (deviceParameterTemplate == null)
						{
							deviceParameterTemplate = new DeviceParameterTemplate()
							{
								Device = new Device()
								{
									DriverUID = driver.UID,
									Driver = driver
								}
							};
							parameterTemplate.DeviceParameterTemplates.Add(deviceParameterTemplate);
						}

						var properties = new List<Property>();
						foreach (var driverProperty in driver.Properties)
						{
							if (driverProperty.IsAUParameter)
							{
								var property = deviceParameterTemplate.Device.SystemAUProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
								if (property == null)
								{
									property = new Property()
									{
										Name = driverProperty.Name,
										Value = driverProperty.Default
									};
									deviceParameterTemplate.Device.SystemAUProperties.Add(property);
								}
								property.DriverProperty = driverProperty;
								properties.Add(property);
							}
						}
						deviceParameterTemplate.Device.SystemAUProperties = properties;
					}
				}
			}
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		public override void OnShow()
		{
			base.OnShow();
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}
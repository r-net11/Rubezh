using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ParameterTemplatesViewModel : MenuViewPartViewModel 
	{
		public ParameterTemplatesViewModel()
		{
			Menu = new ParameterTemplatesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Invalidate();
			ParameterTemplates = new ObservableCollection<ParameterTemplateViewModel>();
			foreach (var parameterTemplate in GKManager.ParameterTemplates)
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
				OnPropertyChanged(() => ParameterTemplates);
			}
		}

		ParameterTemplateViewModel _selectedParameterTemplate;
		public ParameterTemplateViewModel SelectedParameterTemplate
		{
			get { return _selectedParameterTemplate; }
			set
			{
				_selectedParameterTemplate = value;
				OnPropertyChanged(() => SelectedParameterTemplate);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var maxNo = GKManager.ParameterTemplates.Max(x => x.No);
			var parameterTemplate = new GKParameterTemplate()
			{
				Name = "Шаблон " + (maxNo + 1).ToString(),
				No = maxNo + 1
			};
			GKManager.ParameterTemplates.Add(parameterTemplate);
			Invalidate();
			var parameterTemplateViewModel = new ParameterTemplateViewModel(parameterTemplate);
			ParameterTemplates.Add(parameterTemplateViewModel);
			SelectedParameterTemplate = parameterTemplateViewModel;
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool CanDelete()
		{
			return SelectedParameterTemplate != null && SelectedParameterTemplate.ParameterTemplate.Name != "По умолчанию";
		}
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = ParameterTemplates.IndexOf(SelectedParameterTemplate);
			GKManager.ParameterTemplates.RemoveAll(x => x.UID == SelectedParameterTemplate.ParameterTemplate.UID);
			ParameterTemplates.Remove(SelectedParameterTemplate);
			index = Math.Min(index, ParameterTemplates.Count - 1);
			if (index > -1)
				SelectedParameterTemplate = ParameterTemplates[index];
			ServiceFactory.SaveService.GKChanged = true;
		}
		public RelayCommand EditCommand { get; private set; }

		void Invalidate()
		{
			if (GKManager.ParameterTemplates.Count == 0)
			{
				var parameterTemplate = new GKParameterTemplate()
				{
					Name = "По умолчанию"
				};
				GKManager.ParameterTemplates.Add(parameterTemplate);
			}

			foreach (var parameterTemplate in GKManager.ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
					deviceParameterTemplate.GKDevice.Driver = GKManager.Drivers.FirstOrDefault(x => x.UID == deviceParameterTemplate.GKDevice.DriverUID);
				}
				parameterTemplate.DeviceParameterTemplates.RemoveAll(x => x.GKDevice.Driver == null);

				foreach (var driver in GKManager.Drivers)
				{
					if (driver.IsReal && driver.Properties.Any(x => x.IsAUParameter))
					{
						var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.GKDevice.DriverUID == driver.UID);
						if (deviceParameterTemplate == null)
						{
							deviceParameterTemplate = new GKDeviceParameterTemplate()
							{
								GKDevice = new GKDevice()
								{
									DriverUID = driver.UID,
									Driver = driver
								}
							};
							parameterTemplate.DeviceParameterTemplates.Add(deviceParameterTemplate);
						}

						var properties = new List<GKProperty>();
						foreach (var driverProperty in driver.Properties)
						{
							if (driverProperty.IsAUParameter)
							{
								var property = deviceParameterTemplate.GKDevice.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
								if (property == null || parameterTemplate.Name == "По умолчанию")
								{
									property = new GKProperty()
									{
										Name = driverProperty.Name,
										Value = driverProperty.Default,
										DriverProperty = driverProperty
									};
									deviceParameterTemplate.GKDevice.Properties.Add(property);
								}
								property.DriverProperty = driverProperty;
								properties.Add(property);
							}
						}
						deviceParameterTemplate.GKDevice.Properties = properties;
					}
				}
			}
		}
		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}
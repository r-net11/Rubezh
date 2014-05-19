using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class ParameterTemplatesViewModel : MenuViewPartViewModel, IEditingViewModel
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
			foreach (var parameterTemplate in XManager.ParameterTemplates)
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
			var maxNo = XManager.ParameterTemplates.Max(x => x.No);
			var parameterTemplate = new XParameterTemplate()
			{
				Name = "Шаблон " + (maxNo + 1).ToString(),
				No = maxNo + 1
			};
			XManager.ParameterTemplates.Add(parameterTemplate);
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
			XManager.ParameterTemplates.RemoveAll(x => x.UID == SelectedParameterTemplate.ParameterTemplate.UID);
			ParameterTemplates.Remove(SelectedParameterTemplate);
			SelectedParameterTemplate = ParameterTemplates.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;
		}
		public RelayCommand EditCommand { get; private set; }

		void Invalidate()
		{
			if (XManager.ParameterTemplates.Count == 0)
			{
				var parameterTemplate = new XParameterTemplate()
				{
					Name = "По умолчанию"
				};
				XManager.ParameterTemplates.Add(parameterTemplate);
			}

			foreach (var parameterTemplate in XManager.ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
					deviceParameterTemplate.XDevice.Driver = XManager.Drivers.FirstOrDefault(x => x.UID == deviceParameterTemplate.XDevice.DriverUID);
				}
				parameterTemplate.DeviceParameterTemplates.RemoveAll(x => x.XDevice.Driver == null);

				foreach (var driver in XManager.Drivers)
				{
					if (driver.Properties.Any(x => x.IsAUParameter))
					{
						var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.XDevice.DriverUID == driver.UID);
						if (deviceParameterTemplate == null)
						{
							deviceParameterTemplate = new XDeviceParameterTemplate()
							{
								XDevice = new XDevice()
								{
									DriverUID = driver.UID,
									Driver = driver
								}
							};
							parameterTemplate.DeviceParameterTemplates.Add(deviceParameterTemplate);
						}

						var properties = new List<XProperty>();
						foreach (var driverProperty in driver.Properties)
						{
							if (driverProperty.IsAUParameter)
							{
								var property = deviceParameterTemplate.XDevice.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
								if (property == null || parameterTemplate.Name == "По умолчанию")
								{
									property = new XProperty()
									{
										Name = driverProperty.Name,
										Value = driverProperty.Default,
										DriverProperty = driverProperty
									};
									deviceParameterTemplate.XDevice.Properties.Add(property);
								}
								property.DriverProperty = driverProperty;
								properties.Add(property);
							}
						}
						deviceParameterTemplate.XDevice.Properties = properties;
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
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}
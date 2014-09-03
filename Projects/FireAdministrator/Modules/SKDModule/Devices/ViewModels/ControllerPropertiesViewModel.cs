using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.ComponentModel;
using Infrastructure.Events;
using System.Threading;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class ControllerPropertiesViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDDeviceInfo DeviceInfo { get; private set; }
		bool HasChanged { get; set; }

		public ControllerPropertiesViewModel(SKDDevice device, SKDDeviceInfo deviceInfo)
		{
			Title = "Конфигурация контроллера";
			Device = device;
			DeviceInfo = deviceInfo;

			GetControllerDirectionTypeCommand = new RelayCommand(OnGetControllerDirectionType);
			SetControllerDirectionTypeCommand = new RelayCommand(OnSetControllerDirectionType);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime);
			ResetCommand = new RelayCommand(OnReset);
			RebootCommand = new RelayCommand(OnReboot);
			RewriteAllCardsCommand = new RelayCommand(OnRewriteAllCards);

			AvailableControllerDirectionTypes = new ObservableCollection<SKDControllerDirectionType>();
			AvailableControllerDirectionTypes.Add(SKDControllerDirectionType.Unidirect);
			AvailableControllerDirectionTypes.Add(SKDControllerDirectionType.Bidirect);
			SelectedControllerDirectionType = device.ControllerDirectionType;
			HasChanged = false;
		}

		public ObservableCollection<SKDControllerDirectionType> AvailableControllerDirectionTypes { get; private set; }

		SKDControllerDirectionType _selectedControllerDirectionType;
		public SKDControllerDirectionType SelectedControllerDirectionType
		{
			get { return _selectedControllerDirectionType; }
			set
			{
				_selectedControllerDirectionType = value;
				OnPropertyChanged(() => SelectedControllerDirectionType);
				HasChanged = true;
			}
		}

		public RelayCommand GetControllerDirectionTypeCommand { get; private set; }
		void OnGetControllerDirectionType()
		{
			var result = FiresecManager.FiresecService.GetDirectionType(Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				SelectedControllerDirectionType = result.Result;
				HasChanged = false;
			}
		}

		public RelayCommand SetControllerDirectionTypeCommand { get; private set; }
		void OnSetControllerDirectionType()
		{
			var result = FiresecManager.FiresecService.SetDirectionType(Device, SelectedControllerDirectionType);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				HasChanged = false;
			}
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.SKDSyncronyseTime(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции синхронизации времени", result.Error);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			var result = FiresecManager.FiresecService.SKDResetController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RebootCommand { get; private set; }
		void OnReboot()
		{
			var result = FiresecManager.FiresecService.SKDRebootController(Device);
			if (result.Result)
			{
				MessageBoxService.Show("Операция завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции", result.Error);
			}
		}

		public RelayCommand RewriteAllCardsCommand { get; private set; }
		void OnRewriteAllCards()
		{
			var thread = new Thread(() =>
			{
				var result = FiresecManager.FiresecService.SKDRewriteAllCards(Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (result.HasError)
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error);
					}
				}));
			});
			thread.Name = "DeviceCommandsViewModel OnWriteTimeSheduleConfiguration";
			thread.Start();
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				Device.ControllerDirectionType = SelectedControllerDirectionType;
				ServiceFactory.SaveService.SKDChanged = true;
			}

			if (HasChanged)
			{
				if (!MessageBoxService.ShowConfirmation2("Настройки не записаны в прибор. вы уверены, что хотите закрыть окно без записи в прибор?"))
					return false;
			}
			return base.Save();
		}
	}
}
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using StrazhDeviceSDK.API;
using System.Collections.ObjectModel;

namespace ControllerSDK.ViewModels
{
	public class InterlockViewModel : BaseViewModel
	{
		public InterlockViewModel()
		{
			GetInterlockConfigurationCommand = new RelayCommand(OnGetInterlockConfiguration, CanGetInterlockConfiguration);
			SetInterlockConfigurationCommand = new RelayCommand(OnSetInterlockConfiguration, CanSetInterlockConfiguration);
			AvailableInterlockModes = new ObservableCollection<InterlockMode>();
		}

		/// <summary>
		/// Команда чтения настроек для Interlock
		/// </summary>
		public RelayCommand GetInterlockConfigurationCommand { get; private set; }
		void OnGetInterlockConfiguration()
		{
			var cfg = MainViewModel.Wrapper.GetInterlockConfiguration();
			DoorsCount = cfg.DoorsCount;
			CanActivate = cfg.CanActivate;
			IsActivated = cfg.IsActivated;
			InitAvailableInterlockModes(cfg.AvailableInterlockModes);
			SelectedInterlockMode = cfg.CurrentInterlockMode;
		}
		bool CanGetInterlockConfiguration() { return true; }

		/// <summary>
		/// Команда записи настроек для Interlock
		/// </summary>
		public RelayCommand SetInterlockConfigurationCommand { get; private set; }
		void OnSetInterlockConfiguration()
		{
			var cfg = GetModel();
			MainViewModel.Wrapper.SetInterlockConfiguration(cfg);
		}
		bool CanSetInterlockConfiguration() { return true; }

		/// <summary>
		/// Количество дверей на контроллере
		/// </summary>
		int _doorsCount;
		public int DoorsCount
		{
			get { return _doorsCount; }
			set
			{
				_doorsCount = value;
				OnPropertyChanged(() => DoorsCount);
			}
		}

		bool _canActivate;
		public bool CanActivate
		{
			get { return _canActivate; }
			set
			{
				_canActivate = value;
				OnPropertyChanged(() => CanActivate);
			}
		}

		bool _isActivated;
		public bool IsActivated
		{
			get { return _isActivated; }
			set
			{
				_isActivated = value;
				OnPropertyChanged(() => IsActivated);
			}
		}

		public ObservableCollection<InterlockMode> AvailableInterlockModes { get; private set; }
		void InitAvailableInterlockModes(IEnumerable<InterlockModeAvailability> modes)
		{
			AvailableInterlockModes.Clear();
			
			foreach (var mode in modes)
			{
				if (mode.IsAvailable)
				{
					AvailableInterlockModes.Add(mode.InterlockMode);
				}
			}
		}

		InterlockMode _selectedInterlockMode;
		public InterlockMode SelectedInterlockMode
		{
			get { return _selectedInterlockMode; }
			set
			{
				_selectedInterlockMode = value;
				OnPropertyChanged(() => SelectedInterlockMode);
			}
		}

		InterlockConfiguration GetModel()
		{
			var cfg = new InterlockConfiguration();

			cfg.DoorsCount = DoorsCount;
			cfg.IsActivated = IsActivated;
			cfg.CurrentInterlockMode = SelectedInterlockMode;

			return cfg;
		}
	}
}

using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using StrazhDeviceSDK.API;

namespace ControllerSDK.ViewModels
{
	public class AntiPassBackViewModel : BaseViewModel
	{
		public AntiPassBackViewModel()
		{
			GetAntiPassBackConfigurationCommand = new RelayCommand(OnGetAntiPassBackConfiguration, CanGetAntiPassBackConfiguration);
			SetAntiPassBackConfigurationCommand = new RelayCommand(OnSetAntiPassBackConfiguration, CanSetAntiPassBackConfiguration);
			AvailableAntiPassBackModes = new ObservableCollection<AntiPassBackMode>();
		}

		/// <summary>
		/// Команда чтения настроек для Anti-PassBack
		/// </summary>
		public RelayCommand GetAntiPassBackConfigurationCommand { get; private set; }
		void OnGetAntiPassBackConfiguration()
		{
			var cfg = MainViewModel.Wrapper.GetAntiPassBackConfiguration();
			DoorsCount = cfg.DoorsCount;
			CanActivate = cfg.CanActivate;
			IsActivated = cfg.IsActivated;
			InitAvailableAntiPassBackModes(cfg.AvailableAntiPassBackModes);
			SelectedAntiPassBackMode = cfg.CurrentAntiPassBackMode;
		}
		bool CanGetAntiPassBackConfiguration() { return true; }

		/// <summary>
		/// Команда записи настроек для Anti-PassBack
		/// </summary>
		public RelayCommand SetAntiPassBackConfigurationCommand { get; private set; }
		void OnSetAntiPassBackConfiguration()
		{
			var cfg = GetModel();
			MainViewModel.Wrapper.SetAntiPassBackConfiguration(cfg);
		}
		bool CanSetAntiPassBackConfiguration() { return true; }

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

		public ObservableCollection<AntiPassBackMode> AvailableAntiPassBackModes { get; private set; }
		void InitAvailableAntiPassBackModes(IEnumerable<AntiPassBackModeAvailability> modes)
		{
			AvailableAntiPassBackModes.Clear();
			
			foreach (var mode in modes)
			{
				if (mode.IsAvailable)
				{
					AvailableAntiPassBackModes.Add(mode.AntiPassBackMode);
				}
			}
		}

		AntiPassBackMode _selectedAntiPassBackMode;
		public AntiPassBackMode SelectedAntiPassBackMode
		{
			get { return _selectedAntiPassBackMode; }
			set
			{
				_selectedAntiPassBackMode = value;
				OnPropertyChanged(() => SelectedAntiPassBackMode);
			}
		}

		AntiPassBackConfiguration GetModel()
		{
			var cfg = new AntiPassBackConfiguration();

			cfg.DoorsCount = DoorsCount;
			cfg.IsActivated = IsActivated;
			cfg.CurrentAntiPassBackMode = SelectedAntiPassBackMode;

			return cfg;
		}
	}
}

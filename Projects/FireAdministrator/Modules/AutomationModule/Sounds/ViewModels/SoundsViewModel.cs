using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Microsoft.Win32;

namespace AutomationModule.ViewModels
{
	public class SoundsViewModel : MenuViewPartViewModel
	{
		public SoundsViewModel()
		{
			Menu = new SoundsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
		}

		public void Initialize()
		{
			Sounds = new ObservableCollection<SoundViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds = new List<AutomationSound>();
			foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds)
			{
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
			}
			SelectedSound = Sounds.FirstOrDefault();
		}

		ObservableCollection<SoundViewModel> _sounds;
		public ObservableCollection<SoundViewModel> Sounds
		{
			get { return _sounds; }
			set
			{
				_sounds = value;
				OnPropertyChanged("Sounds");
			}
		}

		SoundViewModel _selectedSound;
		public SoundViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				OnPropertyChanged("SelectedSound");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Звуковой файл |*.wav",
				DefaultExt = "Звуковой файл |*.wav"
			};
			if (openFileDialog.ShowDialog().Value)
			{
				var sound = new AutomationSound();
				using (new WaitWrapper())
				{
					sound.Name = openFileDialog.SafeFileName;
					sound.Uid = ServiceFactoryBase.ContentService.AddContent(openFileDialog.FileName);
				}
				FiresecClient.FiresecManager.SystemConfiguration.AutomationSounds.Add(sound);
				ServiceFactory.SaveService.AutomationSoundsChanged = true;
				var soundViewModel = new SoundViewModel(sound);
				Sounds.Add(soundViewModel);
				SelectedSound = soundViewModel;
			}
		}

		bool CanAdd()
		{
			return true;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;

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
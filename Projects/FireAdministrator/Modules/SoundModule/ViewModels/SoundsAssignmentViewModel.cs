using Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SoundsModule.ViewModels
{
	public class SoundsAssignmentViewModel : BaseViewModel
	{
		public SoundsAssignmentViewModel()
		{
			Initialize();
		}

		private void Initialize()
		{
			Sounds = new SortableObservableCollection<SoundAssignmentViewModel>();
			var stateClasses = new List<XStateClass>();

			stateClasses.Add(XStateClass.Attention);
			stateClasses.Add(XStateClass.ConnectionLost);
			stateClasses.Add(XStateClass.Off);
			stateClasses.Add(XStateClass.On);

			var journalEventNameTypes = new List<JournalEventNameType>
			{
				JournalEventNameType.Потеря_связи,
				JournalEventNameType.Восстановление_связи,
				JournalEventNameType.Дверь_не_закрыта_начало,
				JournalEventNameType.Взлом,
				JournalEventNameType.Повторный_проход,
				JournalEventNameType.Принуждение,
				JournalEventNameType.Вскрытие_контроллера_начало,
				JournalEventNameType.Местная_тревога_начало
			};

			foreach (var journalEventNameType in journalEventNameTypes)
			{
				var newSound = new Sound
				{
					JournalEventNameType = journalEventNameType
				};

				var sound = FiresecClient.FiresecManager.SystemConfiguration.Sounds.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (sound == null)
					FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
				else
					newSound = sound;

				Sounds.Add(new SoundAssignmentViewModel(newSound));
			}

			SelectedSound = Sounds.FirstOrDefault();

			if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.RemoveAll(x => Sounds.All(y => y.JournalEventNameType != x.JournalEventNameType)) > 0)
				ServiceFactory.SaveService.SoundsChanged = true;
		}

		private SortableObservableCollection<SoundAssignmentViewModel> _sounds;
		public SortableObservableCollection<SoundAssignmentViewModel> Sounds
		{
			get { return _sounds; }
			set
			{
				_sounds = value;
				OnPropertyChanged(() => Sounds);
			}
		}

		private SoundAssignmentViewModel _selectedSound;
		public SoundAssignmentViewModel SelectedSound
		{
			get { return _selectedSound; }
			set
			{
				_selectedSound = value;
				OnPropertyChanged(() => SelectedSound);
			}
		}
	}
}
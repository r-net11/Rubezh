using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace SoundsModule.ViewModels
{
	public class SoundAssignmentViewModel : BaseViewModel
	{
		private Dictionary<string, SoundLibraryType> _soundsDictionary;

		public const string DefaultName = "<нет>";

		public Sound Sound { get; set; }

		public SoundAssignmentViewModel(Sound sound)
		{
			_soundsDictionary = new Dictionary<string, SoundLibraryType>();
			Sound = sound;
		}

		public JournalEventNameType JournalEventNameType
		{
			get { return Sound.JournalEventNameType; }
		}

		public XStateClass StateClass
		{
			get { return Sound.StateClass; }
		}

		public string SoundName
		{
			get { return Sound.SoundName; }
			set
			{
				Sound.SoundName = value;
				Sound.SoundLibraryType = _soundsDictionary[value];
				OnPropertyChanged(() => SoundName);
				ServiceFactory.SaveService.SoundsChanged = true;
			}
		}

		public bool IsContinious
		{
			get { return Sound.IsContinious; }
			set
			{
				Sound.IsContinious = value;
				OnPropertyChanged(() => IsContinious);
				ServiceFactory.SaveService.SoundsChanged = true;
			}
		}

		public List<string> AvailableSounds
		{
			get
			{
				_soundsDictionary.Clear();

				_soundsDictionary.Add(string.Empty, SoundLibraryType.None);
				
				// Системные звуки
				FiresecClient.FileHelper.SoundsList.ForEach(ss => _soundsDictionary.Add(ss, SoundLibraryType.System));
				
				// Пользовательские звуки
				if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds != null)
				{
					FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.ForEach(us => _soundsDictionary.Add(us.Name, SoundLibraryType.User));
				}

				return _soundsDictionary.Keys.ToList();
			}
		}
	}
}
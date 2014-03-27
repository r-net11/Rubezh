using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System;

namespace GKModule.ViewModels
{
	public class DelayDetailsViewModel : SaveCancelDialogViewModel
	{
		public XDelay Delay { get; private set; }

		public DelayDetailsViewModel(XDelay delay = null)
		{
			if (delay == null)
			{
				Title = "Создание новой задержки";

				Delay = new XDelay()
				{
					Name = "Задержка",
					No = 1,
				};
				if (XManager.Delays.Count != 0)
					Delay.No = (ushort)(XManager.Delays.Select(x => x.No).Max() + 1);
			}
			else
			{
				Title = string.Format("Свойства задержки: {0}", delay.PresentationName);
				Delay = delay;
			}
			CopyProperties();

			var availableNames = new HashSet<string>();
			var availableDescription = new HashSet<string>();
			foreach (var existingDelay in XManager.Delays)
			{
				availableNames.Add(existingDelay.Name);
			}
			AvailableNames = new ObservableCollection<string>(availableNames);

			DelayRegimes = Enum.GetValues(typeof(DelayRegime)).Cast<DelayRegime>().ToList();
		}

		void CopyProperties()
		{
			No = Delay.No;
			Name = Delay.Name;
			Description = Delay.Description;
			DelayTime = Delay.DelayTime;
			Hold = Delay.Hold;
			DelayRegime = Delay.DelayRegime;
		}

		int _no;
		public int No
		{
			get { return _no; }
			set
			{
				_no = value;
				OnPropertyChanged("No");
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		ushort _delayTime;
		public ushort DelayTime
		{
			get { return _delayTime; }
			set
			{
				_delayTime = value;
				OnPropertyChanged("DelayTime");
			}
		}

		ushort _hold;
		public ushort Hold
		{
			get { return _hold; }
			set
			{
				_hold = value;
				OnPropertyChanged("Hold");
			}
		}

		public List<DelayRegime> DelayRegimes { get; private set; }

		DelayRegime _delayRegime;
		public DelayRegime DelayRegime
		{
			get { return _delayRegime; }
			set
			{
				_delayRegime = value;
				OnPropertyChanged("DelayRegime");
			}
		}

		public ObservableCollection<string> AvailableNames { get; private set; }

		protected override bool Save()
		{
			if (XManager.Delays.Any(x => x.Name == Name && x.BaseUID != Delay.BaseUID))
			{
				MessageBoxService.Show("Задержка с таким названием уже существует");
				return false;
			}

			Delay.No = No;
			Delay.Name = Name;
			Delay.Description = Description;
			Delay.DelayTime = DelayTime;
			Delay.Hold = Hold;
			Delay.DelayRegime = DelayRegime;
			return base.Save();
		}
	}
}
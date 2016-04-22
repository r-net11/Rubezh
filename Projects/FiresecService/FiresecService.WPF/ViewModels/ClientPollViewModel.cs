using Infrastructure.Common.Windows.ViewModels;
using System;

namespace FiresecService.Models
{
	public class ClientPollViewModel : BaseViewModel
	{
		public string Client { get; set; }
		public Guid UID { get; set; }

		DateTime _firstPollTime;
		public DateTime FirstPollTime
		{
			get { return _firstPollTime; }
			set
			{
				_firstPollTime = value;
				OnPropertyChanged(() => FirstPollTime);
			}
		}

		DateTime _lastPollTime;
		public DateTime LastPollTime
		{
			get { return _lastPollTime; }
			set
			{
				_lastPollTime = value;
				OnPropertyChanged(() => LastPollTime);
			}
		}

		int _callbackIndex;
		public int CallbackIndex
		{
			get { return _callbackIndex; }
			set
			{
				_callbackIndex = value;
				OnPropertyChanged(() => CallbackIndex);
			}
		}
	}
}
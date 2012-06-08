using System;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace FiresecService.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public FiresecService.Service.FiresecService FiresecService { get; set; }
		public Guid UID { get; set; }
		public string IpAddress { get; set; }
		public string CallbackAddress { get; set; }
		public ClientType ClientType { get; set; }
		public DateTime ConnectionDate { get; set; }
		public List<string> Operations { get; set; }

		public ClientViewModel()
		{
			Operations = new List<string>();
		}

		string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				OnPropertyChanged("UserName");
			}
		}

		string _currentOperationName;
		public string CurrentOperationName
		{
			get { return _currentOperationName; }
			set
			{
				_currentOperationName = value;
				OnPropertyChanged("CurrentOperationName");
			}
		}

		string _callbackOperationName;
		public string CallbackOperationName
		{
			get { return _callbackOperationName; }
			set
			{
				_callbackOperationName = value;
				OnPropertyChanged("CallbackOperationName");
			}
		}

		string _state;
		public string State
		{
			get { return _state; }
			set
			{
				_state = value;
				OnPropertyChanged("State");
			}
		}
	}
}
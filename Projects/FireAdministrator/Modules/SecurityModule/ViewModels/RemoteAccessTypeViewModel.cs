using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SecurityModule.Events;

namespace SecurityModule.ViewModels
{
	public class RemoteAccessTypeViewModel : BaseViewModel
	{
		public RemoteAccessTypeViewModel(RemoteAccessType remoteAccessType)
		{
			RemoteAccessType = remoteAccessType;
		}

		public RemoteAccessType RemoteAccessType { get; private set; }

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				if (_isActive)
					ServiceFactory.Events.GetEvent<RemoteAccessTypeChecked>().Publish(this);

				OnPropertyChanged(() => IsActive);
			}
		}
	}
}
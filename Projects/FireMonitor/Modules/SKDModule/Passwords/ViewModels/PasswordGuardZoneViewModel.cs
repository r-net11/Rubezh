using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class PasswordGuardZoneViewModel : BaseViewModel
	{
		public Password Password { get; private set; }
		public XGuardZone Zone { get; private set; }

		public PasswordGuardZoneViewModel(Password password, XGuardZone zone)
		{
			Password = password;
			Zone = zone;
			_isChecked = Password != null && Password.GuardZoneUIDs.Contains(zone.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
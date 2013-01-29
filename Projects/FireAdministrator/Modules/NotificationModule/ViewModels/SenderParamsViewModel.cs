using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class SenderParamsViewModel : BaseViewModel
	{
		public SenderParamsViewModel()
		{
			SenderParams = new SenderParams();
		}

		public SenderParamsViewModel(SenderParams senderParams)
		{
			SenderParams = senderParams;
		}

		SenderParams _senderParams;

		public SenderParams SenderParams
		{
			get { return _senderParams; }
			set
			{
				_senderParams = value;
				OnPropertyChanged("SenderParams");
			}
		}

		public void Update()
		{
			OnPropertyChanged("SenderParams");
		}
	}
}
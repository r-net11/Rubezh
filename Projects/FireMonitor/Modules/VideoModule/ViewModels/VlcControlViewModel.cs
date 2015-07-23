using System;
using System.Windows.Threading;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class VlcControlViewModel : BaseViewModel
	{
		public event EventHandler OnStart;
		public event EventHandler OnStop;

		string _rviRTSP;
		public string RviRTSP
		{
			get { return _rviRTSP; }
			set
			{
				_rviRTSP = value;
			}
		}

		public void Start()
		{
			RaiseOnStart();
		}

		public void Stop()
		{
			RaiseOnStop();
		}

		protected virtual void RaiseOnStart()
		{
			if (OnStart == null)
				return;
			var temp = OnStart;
			temp(this, null);
		}

		protected virtual void RaiseOnStop()
		{
			if (OnStop == null)
				return;
			var temp = OnStop;
			temp(this, null);
		}
	}
}

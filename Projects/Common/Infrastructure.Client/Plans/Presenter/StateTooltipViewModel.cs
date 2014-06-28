using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using Common;

namespace Infrastructure.Client.Plans.Presenter
{
	public class StateTooltipViewModel<T> : BaseViewModel
		where T : IStateProvider
	{
		public IDeviceState<XStateClass> State { get; protected set; }
		public T Item { get; protected set; }

		public StateTooltipViewModel(T item)
		{
			Item = item;
			State = item.StateClass;
		}

		public virtual void OnStateChanged()
		{
			OnPropertyChanged(() => Item);
			OnPropertyChanged(() => State);
		}
	}
}

using System;
using System.Collections.Generic;

namespace Infrastructure.Plans.Presenter
{
	public abstract class BaseMonitor<TPlan>
	{
		protected TPlan Plan { get; private set; }
		private List<Action> _callBacks;

		public BaseMonitor(TPlan plan, Action callBack)
			: this(plan)
		{
			AddCallBack(callBack);
		}
		public BaseMonitor(TPlan plan)
		{
			_callBacks = new List<Action>();
			Plan = plan;
		}

		public void AddCallBack(Action action)
		{
			_callBacks.Add(action);
		}
		protected void CallBack()
		{
			_callBacks.ForEach(item => item());
		}
	}
}
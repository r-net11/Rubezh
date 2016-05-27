using System;

namespace StrazhService.Monitor
{
	public class ServiceStateHolder : IServiceStateHolder
	{
		private ServiceState _state;

		#region <Реализация IServiceStateHolder>

		public ServiceState State
		{
			get { return _state; }
			set
			{
				if (_state == value)
					return;
				_state = value;
				RaiseServiceStateChanged();
			}
		}

		public event Action<ServiceState> ServiceStateChanged;

		#endregion </Реализация IServiceStateHolder>

		protected virtual void RaiseServiceStateChanged()
		{
			var temp = ServiceStateChanged;
			if (temp != null)
				temp(_state);
		}
	}
}
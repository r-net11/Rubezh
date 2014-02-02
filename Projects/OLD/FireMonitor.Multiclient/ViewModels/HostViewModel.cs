using System;
using System.Windows;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using MuliclientAPI;

namespace FireMonitor.Multiclient.ViewModels
{
	public class HostViewModel : BaseViewModel
	{
		public MulticlientData MulticlientData { get; private set; }
		MulticlientControllerWrapper _controller;
		public event Action<HostViewModel> StateTypeChanged;

		public HostViewModel(MulticlientData multiclientData)
			: base()
		{
			MulticlientData = multiclientData;
			_controller = new MulticlientControllerWrapper(multiclientData.Id);
			_controller.ControlChanged += new EventHandler(ControlChanged);
			_controller.StateTypeChanged += new Action<FiresecAPI.StateType>(Controller_StateChanged);
			_controller.Start(multiclientData);
		}

		void Controller_StateChanged(StateType stateType)
		{
			StateType = stateType;
			if (StateTypeChanged != null)
				StateTypeChanged(this);
		}

		public bool IsReady
		{
			get { return _controller.Contract != null; }
		}

		private void ControlChanged(object sender, EventArgs e)
		{
			ApplicationService.Invoke(() =>
				{
					CommandManager.InvalidateRequerySuggested();
					HostControl = _controller.GetContent();
					OnPropertyChanged(() => HostControl);
				});
		}

		public FrameworkElement HostControl { get; private set; }
		public string Caption
		{
			get { return MulticlientData.Name; }
		}

		StateType _stateType = StateType.Unknown;
		public StateType StateType
		{
			get { return _stateType; }
			set
			{
				_stateType = value;
				OnPropertyChanged("StateType");
			}
		}
	}
}
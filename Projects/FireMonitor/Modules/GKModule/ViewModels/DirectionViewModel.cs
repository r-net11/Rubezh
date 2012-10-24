using System.Collections.Generic;
using Common.GK;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Text;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public XDirectionState DirectionState { get; private set; }
		public XDirection Direction
		{
			get { return DirectionState.Direction; }
		}

		public DirectionViewModel(XDirectionState directionState)
		{
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetAutomaticCommand = new RelayCommand(OnSetAutomatic, CanSetAutomatic);
			ResetAutomaticCommand = new RelayCommand(OnResetAutomatic, CanResetAutomatic);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffNowCommand = new RelayCommand(OnTurnOffNow);

			DirectionState = directionState;
			DirectionState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("DirectionState");
			OnPropertyChanged("ToolTip");
		}

		public string ToolTip
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Direction.PresentationName);
				stringBuilder.AppendLine("Состояние: " + DirectionState.StateType.ToDescription());
				foreach (var stateType in DirectionState.States)
				{
					stringBuilder.AppendLine(stateType.ToDescription());
				}
				return stringBuilder.ToString();
			}
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			SendControlCommand(0x86);
		}
		bool CanSetIgnore()
		{
			return !DirectionState.States.Contains(XStateType.Ignore);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			SendControlCommand(0x06);
		}
		bool CanResetIgnore()
		{
			return DirectionState.States.Contains(XStateType.Ignore);
		}

		public RelayCommand SetAutomaticCommand { get; private set; }
		void OnSetAutomatic()
		{
			SendControlCommand(0x80);
		}
		bool CanSetAutomatic()
		{
			return !Direction.DirectionState.States.Contains(XStateType.Norm);
		}

		public RelayCommand ResetAutomaticCommand { get; private set; }
		void OnResetAutomatic()
		{
			SendControlCommand(0x00);
		}
		bool CanResetAutomatic()
		{
			return Direction.DirectionState.States.Contains(XStateType.Norm);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			SendControlCommand(0x8b);
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			SendControlCommand(0x8d);
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			SendControlCommand(0x90);
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			SendControlCommand(0x91);
		}

		void SendControlCommand(byte code)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes(Direction.GetDatabaseNo(DatabaseType.Gk)));
			bytes.Add(code);
			SendManager.Send(Direction.GkDatabaseParent, 3, 13, 0, bytes);
		}
	}
}
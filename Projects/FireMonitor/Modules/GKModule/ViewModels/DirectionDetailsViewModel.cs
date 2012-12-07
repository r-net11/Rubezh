using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using XFiresecAPI;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace GKModule.ViewModels
{
    public class DirectionDetailsViewModel : DialogViewModel, IWindowIdentity
    {
        public XDirection Direction { get; private set; }
        public XDirectionState DirectionState { get; private set; }

        public DirectionDetailsViewModel(XDirection direction)
        {
            Direction = direction;
            DirectionState = Direction.DirectionState;
            DirectionState.StateChanged += new Action(OnStateChanged);

            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);

            Title = Direction.PresentationName;
            TopMost = true;
        }

        void OnStateChanged()
        {
            OnPropertyChanged("DirectionState");
        }

        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (DirectionState.States.Contains(XStateType.Ignore))
                    return DeviceControlRegime.Ignore;

                if (DirectionState.States.Contains(XStateType.Norm))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }

        void SendControlCommand(byte code)
        {
            ObjectCommandSendHelper.SendControlCommand(Direction, code);
        }

        public RelayCommand SetAutomaticStateCommand { get; private set; }
        void OnSetAutomaticState()
        {
            SendControlCommand(0x80);
            SendControlCommand(0x06);
        }

        public RelayCommand SetManualStateCommand { get; private set; }
        void OnSetManualState()
        {
            SendControlCommand(0x00);
            SendControlCommand(0x06);
        }

        public RelayCommand SetIgnoreStateCommand { get; private set; }
        void OnSetIgnoreState()
        {
            SendControlCommand(0x86);
            SendControlCommand(0x00);
        }

        #region IWindowIdentity Members
        public string Guid
        {
            get { return Direction.UID.ToString(); }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DeviceState
    {
        public DeviceState()
        {
            ParentStates = new List<ParentDeviceState>();
            States = new List<DeviceDriverState>();
        }

        public Device Device { get; set; }
        public string PlaceInTree { get; set; }
        public ChangeEntities ChangeEntities { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public List<DeviceDriverState> States { get; set; }

        [DataMember]
        public List<ParentDeviceState> ParentStates { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        public StateType StateType
        {
            get { return (StateType)StateClassId; }

        }

        public int StateClassId
        {
            get
            {
                List<int> stateClassIds = new List<int>() { 7 };

                stateClassIds.AddRange(from DeviceDriverState deviceDriverState in States
                                       where deviceDriverState.IsActive
                                       select deviceDriverState.DriverState.StateClassId);

                stateClassIds.AddRange(from ParentDeviceState parentDeviceState in ParentStates
                                       select parentDeviceState.DriverState.StateClassId);

                return stateClassIds.Min();
            }
        }

        public List<string> ParentStringStates
        {
            get
            {
                List<string> parentStringStates = new List<string>();
                foreach (var parentDeviceState in ParentStates)
                {
                    parentStringStates.Add(parentDeviceState.ParentDevice.Driver.ShortName + " - " + parentDeviceState.DriverState.Name);
                }
                return parentStringStates;
            }
        }

        public bool IsDisabled
        {
            get { return States.Any(x => ((x.IsActive) && (x.DriverState.StateType == StateType.Off))); }
        }

        public void CopyFrom(DeviceState deviceState)
        {
            Id = deviceState.Id;
            States = deviceState.States;
            ParentStates = deviceState.ParentStates;
            Parameters = deviceState.Parameters;
        }

        #region Automatic
        bool _isAutomaticOff = false;
        public bool IsAutomaticOff
        {
            get { return _isAutomaticOff; }
            set
            {
                if (_isAutomaticOff != value)
                {
                    _isAutomaticOff = value;
                    if (value)
                        AlarmAdded(AlarmType.Auto, Id);
                    else
                        AlarmRemoved(AlarmType.Auto, Id);
                }
            }
        }

        bool _isOff = false;
        public bool IsOff
        {
            get { return _isOff; }
            set
            {
                if (_isOff != value)
                {
                    _isOff = value;
                    if (value)
                        AlarmAdded(AlarmType.Off, Id);
                    else
                        AlarmRemoved(AlarmType.Off, Id);
                }
            }
        }

        bool _isFailure = false;
        public bool IsFailure
        {
            get { return _isFailure; }
            set
            {
                if (_isFailure != value)
                {
                    _isFailure = value;
                    if (value)
                        AlarmAdded(AlarmType.Failure, Id);
                    else
                        AlarmRemoved(AlarmType.Failure, Id);
                }
            }
        }

        bool _isFire = false;
        public bool IsFire
        {
            get { return _isFire; }
            set
            {
                if (_isFire != value)
                {
                    _isFire = value;
                    if (value)
                        AlarmAdded(AlarmType.Fire, Id);
                    else
                        AlarmRemoved(AlarmType.Fire, Id);
                }
            }
        }

        bool _isAttention = false;
        public bool IsAttention
        {
            get { return _isAttention; }
            set
            {
                if (_isAttention != value)
                {
                    _isAttention = value;
                    if (value)
                        AlarmAdded(AlarmType.Attention, Id);
                    else
                        AlarmRemoved(AlarmType.Attention, Id);
                }
            }
        }

        bool _isInfo = false;
        public bool IsInfo
        {
            get { return _isInfo; }
            set
            {
                if (_isInfo != value)
                {
                    _isInfo = value;
                    if (value)
                        AlarmAdded(AlarmType.Info, Id);
                    else
                        AlarmRemoved(AlarmType.Info, Id);
                }
            }
        }

        bool _isService = false;
        public bool IsService
        {
            get { return _isService; }
            set
            {
                if (_isService != value)
                {
                    _isService = value;
                    if (value)
                        AlarmAdded(AlarmType.Service, Id);
                    else
                        AlarmRemoved(AlarmType.Service, Id);
                }
            }
        }

        public static event Action<AlarmType, string> AlarmAdded;
        static void OnAlarmAdded(AlarmType alarmType, string id)
        {
            if (AlarmAdded != null)
                AlarmAdded(alarmType, id);
        }

        public static event Action<AlarmType, string> AlarmRemoved;
        static void OnAlarmRemoved(AlarmType alarmType, string id)
        {
            if (AlarmRemoved != null)
                AlarmRemoved(alarmType, id);
        }
        #endregion Automatic

        public event Action StateChanged;
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged();
        }
    }
}
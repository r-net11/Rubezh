using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.OSI.ApplicationLayer
{
	public class ParameterChangedEventArgs : EventArgs
	{
		Guid _deviceId;
		string _parameterName;
		ValueType _newValue;

		public Guid DeviceId
		{
			get { return _deviceId; }
			set { _deviceId = value; }
		}

		public string ParameterName
		{
			get { return _parameterName; }
			set { _parameterName = value; }
		}

		public ValueType NewValue
		{
			get { return _newValue; }
			set { _newValue = value; }
		}

		public ParameterChangedEventArgs(Guid deviceId, string parameterName, ValueType newValue)
		{
			_deviceId = deviceId;
			_parameterName = parameterName;
			_newValue = newValue;
		}
	}
}

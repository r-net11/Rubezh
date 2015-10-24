using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.ApplicationLayer
{
	public enum ConfigurationChangedAction
	{
		/// <summary>
		/// Добавлено устройство
		/// </summary>
		DeviceAdded,
		/// <summary>
		/// Устройтство удалено
		/// </summary>
		DeviceRemoved,
		/// <summary>
		/// Устройство собираются удалить
		/// </summary>
		DeviceRemoving
	}

	/// <summary>
	/// Ар
	/// </summary>
	public class ConfigurationChangedEventArgs: EventArgs
	{
		#region Fields And Properties

		IDevice _device;
		ConfigurationChangedAction _action;

		/// <summary>
		/// Устройство
		/// </summary>
		public IDevice Device
		{
			get { return _device; }
			set { _device = value; }
		}

		/// <summary>
		/// Действие с элементом конфигурации
		/// </summary>
		public ConfigurationChangedAction Action
		{
			get { return _action; }
			set { _action = value; }
		}

		#endregion
	}
}

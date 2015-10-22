using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		DeviceRemoved
	}

	/// <summary>
	/// Ар
	/// </summary>
	public class ConfigurationChangedEventArgs: EventArgs
	{
		#region Fields And Properties

		Guid _id;
		ConfigurationChangedAction _action;

		/// <summary>
		/// Идентификатор элемента конфигурации
		/// </summary>
		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
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

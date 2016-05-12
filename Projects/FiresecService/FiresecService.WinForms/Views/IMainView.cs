using RubezhAPI.License;
using System;
using System.Windows.Forms;

namespace FiresecService.Views
{
	/// <summary>
	/// Закладки окна
	/// </summary>
	//public enum MainViewTabs
	//{
	//	Connections,
	//	Log,
	//	Status,
	//	GK,
	//	Polling,
	//	Operations,
	//	Licence
	//}

	public interface IMainView
	{
		/// <summary>
		/// Заголовок окна
		/// </summary>
		string Title { set; }
		/// <summary>
		/// Status bar: Последение событие сервера
		/// </summary>
		string LastLog { get; set; }
		/// <summary>
		/// Событие происходит при вызове контекстного меню "Разорвать соединение"
		/// </summary>
		event EventHandler CommandDisconnectActivated;
		/// <summary>
		/// Разрешает или запрещает контекстноем меню "Разоравть соединение"
		/// </summary>
		bool EnableMenuDisconnect { get; set; }
		BindingSource ClientsContext { set; }
		BindingSource LogsContext { set; }
		string RemoteAddress { set; }
		BindingSource GkLifecyclesContext { set; }
		BindingSource ClientPollsContext { set; }
		BindingSource OperationsContext { set; }

		LicenseMode LicenseMode { set; }
		int RemoteClientsCount { set; }
		bool HasFirefighting { set; }
		bool HasGuard { set; }
		bool HasSKD { set; }
		bool HasVideo { set; }
		bool HasOpcServer { set; }
		string InitialKey { set; }
		event EventHandler ClickLoadLicense;
	}
}
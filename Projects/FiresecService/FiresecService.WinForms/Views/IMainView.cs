﻿using FiresecService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

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
		string Title { get; set; }
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
		string LocalAddress { set; }
		string RemoteAddress { set; }
		string ReportAddress { set; }
		BindingSource GkLifecyclesContext { set; }
		BindingSource ClientPollsContext { set; }
		BindingSource OperationsContext { set; }
	}
}
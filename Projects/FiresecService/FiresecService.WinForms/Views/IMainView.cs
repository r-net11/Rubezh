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
		/// Текущая выбранная закладка
		/// </summary>
		ITabPageView SelectedTabView { get; }
		/// <summary>
		/// Событие происходит при выборе закладки
		/// </summary>
		event EventHandler TabChanged; 
	}
}
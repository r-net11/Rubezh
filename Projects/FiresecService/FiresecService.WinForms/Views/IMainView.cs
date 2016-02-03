using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FiresecService.Views
{
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
		/// Tray: Выводит сообщение
		/// </summary>
		/// <param name="timeOut">Время отображения сообщения, мсек</param>
		/// <param name="title">Заголовок сообщения</param>
		/// <param name="text">Текс сообщения</param>
		/// <param name="icon">Иконка сообщения</param>
		void ShowBalloonTip(int timeOut, string title, string text, ToolTipIcon icon);
	}
}
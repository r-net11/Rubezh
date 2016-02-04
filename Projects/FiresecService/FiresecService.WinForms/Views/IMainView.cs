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
	}
}
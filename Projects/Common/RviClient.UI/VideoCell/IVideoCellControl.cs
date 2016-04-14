using System;
using RviClient.UI.VideoCell;

namespace RviClient.UI
{
	public interface IVideoCellControl
	{
		/// <summary>
		/// Панель, на которой размещен плеер
		/// </summary>
		PlayerPanel PlayerPanel { get; }

		/// <summary>
		/// Проигрыватель
		/// </summary>
		MediaSourcePlayer.MediaPlayer MediaPlayer { get; }

		/// <summary>
		/// Всегда показывать инструментальные панели
		/// </summary>
		bool AlwaysShowToolbars { get; set; }

		/// <summary>
		/// Заголовок, отображаемый на верхней всплывающей панели
		/// </summary>
		string TopToolbarTitle { get; set; }

		/// <summary>
		/// Заголовок, отображаемый на нижней всплывающей панели
		/// </summary>
		string BottomToolbarTitle { get; set; }

		/// <summary>
		/// Показать кнопку "Перазапустить"
		/// </summary>
		bool ShowReconnectButton { get; set; }

		/// <summary>
		/// Событие возникает, когда нажата клавиша "Перезапустить"
		/// </summary>
		event EventHandler ReconnectEvent;
	}
}
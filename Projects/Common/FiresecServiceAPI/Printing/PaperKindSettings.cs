using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrazhAPI.Printing
{
	/// <summary>
	/// Объект, описывающий структуру хранения настроек для разновидности формата листа
	/// </summary>
	public class PaperKindSettings
	{
		public string Name { get; set; }
		/// <summary>
		/// Ширина в миллиметрах
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// Высота в миллиметрах
		/// </summary>
		public int Height { get; set; }
	}
}

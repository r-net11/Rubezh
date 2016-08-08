using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrazhAPI.Printing
{
	/// <summary>
	/// Объект, описывающий структуру хранения настроек для разновидности формата листа
	/// </summary>
	internal class UserKindSetting : IPaperKindSetting
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

		public bool IsSystem { get { return false; } }

		public UserKindSetting()
		{
			Name = "Пользовательский размер листа";
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

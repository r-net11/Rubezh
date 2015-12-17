using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpcFoundation
{
	public class OpcDaDirectory: OpcDaItemBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="directoryName">null - корневая директория</param>
		/// <param name="parent">директория, в которую будет вложена данная директория.
		/// Для корневой директории всегда равна null</param>
		public OpcDaDirectory(string directoryName = null, OpcDaDirectory parent = null)
		{
			_directory = parent;
			
			if ((parent != null) && (String.IsNullOrEmpty(directoryName)))
			{
				throw new ArgumentException(
					"Попытка присвоить не корневой директории пустое имя",
					"directoryName");
			}
			else if ((parent == null) && (directoryName != null)) 
			{
				throw new ArgumentException(
					"Попытка присвоить корневой директории значение не равное null",
					"directoryName");				
			}
			else
			{
				_directoryName = String.IsNullOrEmpty(directoryName) ? RootDirectory : directoryName;
			}
		}

		public string DirectoryName
		{
			get { return _directoryName; }
		}

		public override bool IsDirectory
		{
			get { return true; }
		}

		public bool IsRoot
		{
			get { return _directoryName == RootDirectory; }
		}

		List<OpcDaItemBase> _items = new List<OpcDaItemBase>();

		/// <summary>
		/// Список воложенных тегов и директорий
		/// </summary>
		public IList<OpcDaItemBase> Items { get { return _items; } }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpcDaServer
{
	public class OpcDaTag: OpcDaItemBase
	{
		#region Constructors

		private OpcDaTag() { throw new NotImplementedException(); }

		public OpcDaTag(string tagId, string tagName, OpcDaDirectory directory)
		{
			if (tagId == null)
			{
				throw new ArgumentNullException("tagId");
			}
			if (directory == null)
			{
				throw new ArgumentNullException("directory");
			}
			TagId = tagId;
			Name = tagName == null ? String.Empty : tagName;
			_directory = directory;
		}

		#endregion

		#region Fields And Properties

		public override bool IsDirectory
		{
			get { return false; }
		}

		/// <summary>
		/// UID тега
		/// </summary>
		public string TagId { get; private set; }

		public string Name { get; private set; }

		/// <summary>
		/// Возвращает путь к тегу или директории
		/// </summary>
		public string FullPath
		{ 
			get 
			{
				List<string> segments = new List<string>();
				StringBuilder sb;
				OpcDaDirectory directory;

				segments.Add(Name);
				directory = Directory;
				segments.Add(directory.DirectoryName);

				// Получаем сегменты пути к тегу
				while (!directory.IsRoot)
				{
					directory = directory.Directory;
					segments.Add(directory.DirectoryName);
				} 

				sb = new StringBuilder();
				for (int i = (segments.Count - 1); i >= 0 ; i--)
				{
					sb.Append(segments[i]);
					if (i > 0)
					{
						sb.Append(Spliter);
					}
				}

				return sb.ToString();
			}
		}

		#endregion
	}
}

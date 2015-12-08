using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using OPCDA.NET;

namespace AutomationModule.ViewModels
{
	public abstract class OpcAdvosolElementViewModel : TreeNodeViewModel<OpcAdvosolElementViewModel>
	{
		#region Fields And Properties

		public abstract bool IsTag { get; }
		public abstract string Name { get; protected set; }
		public abstract string Path { get; protected set; }

		#endregion

		/// <summary>
		/// Преобразует дерево тегов OPC сервера в линейный список 
		/// </summary>
		/// <param name="tree"></param>
		/// <returns></returns>
		public static OpcAdvosolElementViewModel[] ConvertTo(BrowseTree tree)
		{
			OpcAdvosolElementViewModel[] result = new OpcAdvosolElementViewModel[0];
			var root = tree.Root();
			foreach (var item in root)
			{
				var s = item.Header;
			}
			return result;
		}
	}
}

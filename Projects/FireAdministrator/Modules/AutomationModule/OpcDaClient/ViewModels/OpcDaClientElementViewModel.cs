using Infrastructure.Common.TreeList;
using RubezhAPI.Automation;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientElementViewModel : TreeNodeViewModel<OpcDaClientElementViewModel>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="element">если null - корневая группа</param>
		public OpcDaClientElementViewModel(OpcDaElement element = null)
		{
			Element = element;
		}

		public OpcDaElement Element { get; private set; }

		public static OpcDaClientElementViewModel Create(OpcDaElement[] elements)
		{
			var root = new OpcDaClientElementViewModel();

			foreach (var element in elements)
			{
				var segments = OpcDaElement.GetGroupNamesAndCheckFormatFromPath(element);

				var currentNode = root;

				for (int i = 1; i < segments.Length; i++) // 0-й индекс корневая группа
				{
					var elm = currentNode.Nodes
						.Select(n => (OpcDaClientElementViewModel)n)
						.FirstOrDefault(node => node.Element.ElementName == segments[i]);

					if (elm == null)
					{
						var node = new OpcDaClientElementViewModel(element);
						currentNode.Nodes.Add(node);
						currentNode = node;
					}
					else
					{
						currentNode = elm;
					}
				}
			}

			return root;
		}

		/// <summary>
		/// Ищет и возвращает корневой элемент (корневая группа тегов) 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		static OpcDaClientElementViewModel GetRootItem(OpcDaClientElementViewModel item)
		{
			OpcDaClientElementViewModel currentElement = item;
			OpcDaClientElementViewModel rootElement = null;

			do
			{
				if ((currentElement.Element == null) && 
					(currentElement.Parent == null) && 
					(!currentElement.Element.IsTag))
				{
					rootElement = currentElement;
				}
				else
				{
					GetRootItem(item.Parent);
					currentElement = item.Parent;
				}
			}
			while (rootElement == null);
			
			return rootElement;
		}
	}
}
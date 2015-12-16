using Infrastructure.Common.TreeList;
using AutomationModule.Models;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientElementViewModel : TreeNodeViewModel<OpcDaClientElementViewModel>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="element">если null - корневая группа</param>
		public OpcDaClientElementViewModel(TsOpcElement element = null)
		{
			Element = element;
		}

		public bool IsTag { get { return Element == null ? false : Element.Element.IsItem; } }
		public string ElementName { get { return Element == null ? TsOpcTagsStructure.ROOT : Element.Element.ItemName; } }
		public TsOpcElement Element { get; private set; }
	}
}
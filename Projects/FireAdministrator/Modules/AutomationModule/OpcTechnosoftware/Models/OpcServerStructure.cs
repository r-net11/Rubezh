using System;
using System.Collections.Generic;
using System.Linq;
using OpcClientSdk.Da;

namespace AutomationModule.Models
{
	public class TsOpcTagsStructure
	{
		#region Constructors

		public TsOpcTagsStructure(TsCDaBrowseElement[] elements)
		{
			var list = elements.Select(x => new TsOpcElement(this, x));
			_allItems = new List<TsOpcElement>(list);
		}

		#endregion

		#region Constants

		public const string ROOT = @".";
		public const string SPLITTER = @"\";
		
		#endregion

		#region Fields And Properties

		List<TsOpcElement> _allItems;

		public TsOpcElement[] AllElements { get { return _allItems.ToArray(); } }

		public TsOpcElement[] Items 
		{
			get
			{
				//var children = GetChildrenInGroup(
				//	new TsCDaBrowseElement { IsItem = false, HasChildren = true, ItemPath = ROOT + SPLITTER });
				//return children.Select(x => new OpcTechnosoftwareElement(this, x)).ToArray();

				var children = GetChildrenInGroup();
				return children;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element">null - корневая группа</param>
		/// <returns></returns>
		public TsOpcElement[] GetChildrenInGroup(TsOpcElement element = null)
		{
			if (element == null)
			{
				element = new TsOpcElement(this,  
					new TsCDaBrowseElement { IsItem = false, HasChildren = true, ItemPath = ROOT + SPLITTER });
			}

			if (element.Element.IsItem)
			{
				// Таг не может содержать вложенных элементов
				return new TsOpcElement[0];
			}

			var result = _allItems.Where(x => x.Element.ItemPath.StartsWith(element.Element.ItemPath) &&
				GetSegmentsOfPath(x.Element).Length == (GetSegmentsOfPath(element.Element).Length + 1)).ToArray();
			return result;
		}

		string[] GetSegmentsOfPath(TsCDaBrowseElement element)
		{
			var result = element.ItemPath.Split(new string[] { SPLITTER }, StringSplitOptions.RemoveEmptyEntries);
			return result;
		}

		#endregion
	}

	public class TsOpcElement
	{
		#region Constructors

		public TsOpcElement(TsOpcTagsStructure owner, 
			TsCDaBrowseElement element)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			_owner = owner;
			_element = element;
		}

		#endregion

		#region Fields And Properties

		TsOpcTagsStructure _owner;
		
		TsCDaBrowseElement _element;
		public TsCDaBrowseElement Element { get { return _element; } }

		public TsOpcElement[] Items
		{
			get 
			{
				return _owner.GetChildrenInGroup(this);
			}
		}

		public bool IsChecked { get; set; }

		#endregion
	}
}
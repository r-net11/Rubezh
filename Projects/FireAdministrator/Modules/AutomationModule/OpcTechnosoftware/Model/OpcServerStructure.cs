using System;
using System.Collections.Generic;
using System.Linq;
using OpcClientSdk.Da;

namespace AutomationModule.Model
{
	public class OpcTechnosoftwareServerStructure
	{
		#region Constructors

		public OpcTechnosoftwareServerStructure(TsCDaBrowseElement[] elements)
		{
			var list = elements.Select(x => new OpcTechnosoftwareElement(this, x));
			_allItems = new List<OpcTechnosoftwareElement>(list);
		}

		#endregion

		#region Constants

		public const string ROOT = @".";
		public const string SPLITTER = @"\";
		
		#endregion

		#region Fields And Properties

		List<OpcTechnosoftwareElement> _allItems;

		public OpcTechnosoftwareElement[] AllElements { get { return _allItems.ToArray(); } }

		public OpcTechnosoftwareElement[] Items 
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
		public OpcTechnosoftwareElement[] GetChildrenInGroup(OpcTechnosoftwareElement element = null)
		{
			if (element == null)
			{
				element = new OpcTechnosoftwareElement(this,  
					new TsCDaBrowseElement { IsItem = false, HasChildren = true, ItemPath = ROOT + SPLITTER });
			}

			if (element.Element.IsItem)
			{
				// Таг не может содержать вложенных элементов
				return new OpcTechnosoftwareElement[0];
			}

			var result = _allItems.Where(x => x.Element.ItemPath.StartsWith(element.Element.ItemPath) &&
				GetSegmentsOfPath(x.Element).Length == (GetSegmentsOfPath(element.Element).Length + 1)).ToArray();
			return result;
		}

		//public TsCDaBrowseElement[] GetChildrenInGroup(TsCDaBrowseElement group)
		//{
		//	if (group.IsItem)
		//	{
		//		// Таг не может содержать вложенных элементов
		//		return new TsCDaBrowseElement[0];
		//	}

		//	var result = _allItems
		//		.Where(x => x.Element.ItemPath.StartsWith(group.ItemPath) &&
		//				GetSegmentsOfPath(x.Element).Length == (GetSegmentsOfPath(group).Length + 1))
		//		.Select(y => y.Element).ToArray();
		//	return result;
		//}

		string[] GetSegmentsOfPath(TsCDaBrowseElement element)
		{
			var result = element.ItemPath.Split(new string[] { SPLITTER }, StringSplitOptions.RemoveEmptyEntries);
			return result;
		}

		#endregion
	}

	public class OpcTechnosoftwareElement
	{
		#region Constructors

		public OpcTechnosoftwareElement(OpcTechnosoftwareServerStructure owner, 
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

		OpcTechnosoftwareServerStructure _owner;
		
		TsCDaBrowseElement _element;
		public TsCDaBrowseElement Element { get { return _element; } }

		public OpcTechnosoftwareElement[] Items
		{
			get 
			{
				//return _owner.GetChildrenInGroup(_element)
				//	.Select(x => new OpcTechnosoftwareElement(_owner, x)).ToArray();
				return _owner.GetChildrenInGroup(this);
			}
		}

		public bool IsChecked { get; set; }

		#endregion
	}
}
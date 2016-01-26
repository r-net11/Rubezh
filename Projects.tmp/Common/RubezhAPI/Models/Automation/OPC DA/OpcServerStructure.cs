using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhAPI.Automation
{
	public class TsOpcTagsStructure
	{
		#region Constructors



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
			return null;
		}

		#endregion
	}

	public class TsOpcElement
	{
		#region Fields And Properties

		TsOpcTagsStructure _owner;
		
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
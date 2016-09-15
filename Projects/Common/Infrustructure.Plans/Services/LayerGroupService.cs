using System.Collections.Generic;
using System.Linq;
using Localization.Common.InfrastructurePlans;

namespace Infrustructure.Plans.Services
{
	public class LayerGroupService : IEnumerable<string>
	{
		private class GroupItem
		{
			public int Order { get; set; }

			public string Name { get; set; }

			public string Alias { get; set; }
		}

		private static LayerGroupService _instance = null;

		public static LayerGroupService Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LayerGroupService();
				return _instance;
			}
		}

		public const string ElementAlias = "Element";
		private Dictionary<string, GroupItem> _groups;
		private Dictionary<string, int> _orders;

		private LayerGroupService()
		{
			_groups = new Dictionary<string, GroupItem>();
			var elementsGroup = new GroupItem()
			{
				Alias = ElementAlias,
				Name = CommonResources.Elements,
				Order = 1000,
			};
			_groups.Add(ElementAlias, elementsGroup);
		}

		public void RegisterGroup(string alias, string name, int order = int.MaxValue)
		{
			var groupItem = new GroupItem()
			{
				Alias = alias,
				Name = name,
				Order = order,
			};
			if (_groups.ContainsKey(alias))
				_groups[alias] = groupItem;
			else
				_groups.Add(alias, groupItem);
		}

		public string this[string alias]
		{
			get { return _groups[alias].Name; }
		}

		public int Count
		{
			get { return _groups.Count; }
		}

		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return _groups.Values.OrderBy(item => item.Order).Select(item => item.Alias).GetEnumerator();
		}

		#endregion IEnumerable<string> Members

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}
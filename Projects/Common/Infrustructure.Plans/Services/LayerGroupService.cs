using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrustructure.Plans.Services
{
	public class LayerGroupService : IEnumerable<string>
	{
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

		public const string ZoneAlias = "Zone";
		public const string SubPlanAlias = "SubPlan";
		public const string ElementAlias = "Element";
		private Dictionary<string, string> _groups = new Dictionary<string, string>();
		private List<string> _order = new List<string>();

		private LayerGroupService()
		{
			_groups.Add(ZoneAlias, "Зоны");
			_groups.Add(SubPlanAlias, "Подпланы");
			_groups.Add(ElementAlias, "Элементы");
			_order.Add(ZoneAlias);
			_order.Add(SubPlanAlias);
			_order.Add(ElementAlias);
		}

		public void RegisterGroup(string alias, string name, int order = -1)
		{
			if (_groups.ContainsKey(alias))
			{
				_groups[alias] = name;
				if (order != -1)
				{
					_order.Remove(alias);
					_order.Insert(order, alias);
				}
			}
			else
			{
				_groups.Add(alias, name);
				if (order == -1)
					_order.Add(alias);
				else
					_order.Insert(order, alias);
			}
		}
		public string this[string alias]
		{
			get { return _groups[alias]; }
		}
		public string this[int index]
		{
			get { return this[_order[index]]; }
		}
		public int Count
		{
			get { return _order.Count; }
		}

		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return _order.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _order.GetEnumerator();
		}

		#endregion
	}
}

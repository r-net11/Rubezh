
using System.Collections.Generic;
using System.Windows;
namespace Common
{
	public class PointCollection : IEnumerable<Point>
	{
		List<Point> _collection;
		public Point this[int index]
		{
			get
			{
				return this._collection[index];
			}
			set
			{
				this._collection[index] = value;
			}
		}

		public PointCollection()
		{
			_collection = new List<Point>();
		}

		public PointCollection(IEnumerable<Point> collection)
		{
			_collection = new List<Point>(collection);
		}

		public int Count
		{
			get
			{
				return this._collection.Count;
			}
		}

		public void Add(Point value)
		{
			_collection.Add(value);
		}

		public void Insert(int index, Point value)
		{
			this._collection.Insert(index, value);
		}

		public bool Remove(Point value)
		{
			return _collection.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this._collection.RemoveAt(index);
		}

		public void Clear()
		{
			this._collection.Clear();
		}

		public IEnumerator<Point> GetEnumerator()
		{
			return _collection.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _collection.GetEnumerator();
		}
	}
}

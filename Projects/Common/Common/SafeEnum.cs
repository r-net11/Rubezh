using System;
using System.Linq;

namespace Common
{
	public struct SafeEnum<T>
	{
		T _value;

		public string StringValue
		{
			get { return _value.ToString(); }
			set
			{
				_value = Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(x => x.ToString() == value);
			}
		}

		SafeEnum(T value)
		{
			_value = value;
			if (!typeof(T).IsEnum)
				throw new InvalidOperationException("Ожидается тип Enum вместо " + typeof(T).FullName);
		}

		#region Operators
		public static implicit operator T(SafeEnum<T> value)
		{
			return value._value;
		}

		public static implicit operator SafeEnum<T>(T value)
		{
			return new SafeEnum<T>(value);
		}

		public static bool operator ==(SafeEnum<T> left, SafeEnum<T> right)
		{
			return left.ToString() == right.ToString();
		}

		public static bool operator !=(SafeEnum<T> left, SafeEnum<T> right)
		{
			return left.ToString() != right.ToString();
		}
		#endregion

		#region Overrided methods
		public override string ToString()
		{
			return _value.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj is SafeEnum<T> ?
				_value.Equals(((SafeEnum<T>)obj)._value) :
				_value.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
		#endregion
	}
}

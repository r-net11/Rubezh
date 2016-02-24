using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Common
{
	public class XmlSerializer<T> where T : class
	{
		private readonly XmlSerializer _serializer;

		public XmlSerializer()
		{
			_serializer = new XmlSerializer(typeof(T));
		}

		public string Serialize(T obj)
		{
			return _serializer.Serialize(obj);
		}

		public void Serialize(T obj, string fileName)
		{
			_serializer.Serialize(obj, fileName);
		}

		public void Serialize(T obj, Stream stream)
		{
			_serializer.Serialize(obj, stream);
		}

		public void Serialize(T obj, XmlWriter xmlWriter)
		{
			_serializer.Serialize(obj, xmlWriter);
		}

		public T DeserializeFromString(string xml)
		{
			return (T)_serializer.DeserializeFromString(xml);
		}

		public T Deserialize(string fileName)
		{
			return (T)_serializer.Deserialize(fileName);
		}

		public T Deserialize(XmlReader xmlReader)
		{
			return (T)_serializer.Deserialize(xmlReader);
		}

		public T Deserialize(Stream stream)
		{
			return (T)_serializer.Deserialize(stream);
		}
	}

	public class XmlSerializer
	{
		private readonly DataContractSerializer _serializer;

		public XmlSerializer(Type type)
		{
			_serializer = new DataContractSerializer(type);
		}

		public string Serialize(object obj)
		{
			var sb = new StringBuilder();
			using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true }))
			{
				_serializer.WriteObject(writer, obj);
			}
			return sb.ToString();
		}

		public void Serialize(object obj, string fileName)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true }))
			{
				_serializer.WriteObject(xmlWriter, obj);
			}
		}

		public void Serialize(object obj, Stream stream)
		{
			_serializer.WriteObject(stream, obj);
			stream.Position = 0;
		}

		public void Serialize(object obj, XmlWriter xmlWriter)
		{
			_serializer.WriteObject(xmlWriter, obj);
		}

		public object DeserializeFromString(string xml)
		{
			object result;
			using (var reader = new XmlTextReader(new StringReader(xml)))
			{
				result = _serializer.ReadObject(reader);
			}
			return result;
		}

		public object Deserialize(string fileName)
		{
			object result;

			using (XmlReader reader = XmlReader.Create(fileName))
			{
				result = _serializer.ReadObject(reader);
			}
			return result;
		}

		public object Deserialize(XmlReader xmlReader)
		{
			return _serializer.ReadObject(xmlReader);
		}

		public object Deserialize(Stream stream)
		{
			stream.Position = 0;

			return _serializer.ReadObject(stream);
		}
	}
}

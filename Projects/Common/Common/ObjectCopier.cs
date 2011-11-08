using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public static class ObjectCopier
    {
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static void CopyTo(this object S, object T)
        {
            foreach (var pS in S.GetType().GetProperties())
            {
                foreach (var pT in T.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name) continue;
                    (pT.GetSetMethod()).Invoke(T, new object[] { pS.GetGetMethod().Invoke(S, null) });
                }
            };
        }

        public static object CloneObject(object opSource)
        {
            //grab the type and create a new instance of that type
            Type opSourceType = opSource.GetType();
            object opTarget = Activator.CreateInstance(opSourceType);

            //grab the properties
            PropertyInfo[] opPropertyInfo = opSourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //iterate over the properties and if it has a 'set' method assign it from the source TO the target
            foreach (PropertyInfo item in opPropertyInfo)
            {
                if (item.CanWrite)
                {
                    //value types can simply be 'set'
                    if (item.PropertyType.IsValueType || item.PropertyType.IsEnum || item.PropertyType.Equals(typeof(System.String)))
                    {
                        item.SetValue(opTarget, item.GetValue(opSource, null), null);
                    }
                    //object/complex types need to recursively call this method until the end of the tree is reached
                    else
                    {
                        object opPropertyValue = item.GetValue(opSource, null);
                        if (opPropertyValue == null)
                        {
                            item.SetValue(opTarget, null, null);
                        }
                        else
                        {
                            item.SetValue(opTarget, CloneObject(opPropertyValue), null);
                        }
                    }
                }
            }
            //return the new item
            return opTarget;
        }

    }    
}

using StrazhAPI.Models.Layouts;
using System;
using System.Runtime.Serialization;
using System.Xml;

namespace StrazhAPI
{
    public class StrazhDataContractResolver : DataContractResolver
    {
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            switch (typeName)
            {
                case "LayoutPartReferenceProperties":
                    return typeof(LayoutPartReferenceProperties);
                case "LayoutPartImageProperties":
                    return typeof(LayoutPartImageProperties);
                case "LayoutPartPlansProperties":
                    return typeof(LayoutPartPlansProperties);
                case "LayoutPartProcedureProperties":
                    return typeof(LayoutPartProcedureProperties);
                case "LayoutPartTimeProperties":
                    return typeof(LayoutPartTimeProperties);
                case "LayoutPartTextProperties":
                    return typeof(LayoutPartTextProperties);
                default:
                    return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
        }

        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            throw new NotImplementedException();
        }
    }
}

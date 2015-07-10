using System.Collections.Generic;
using System.Linq;
using PowerCalculator.Models;
using System.Xml.Serialization;
using System;
using System.IO;

namespace PowerCalculator.Processor
{
	public static class CableTypesRepository
	{
        const string CustomCableTypeName = @"Произвольный";
        public static CableType CustomCableType { get; private set; }
        public static List<CableType> CableTypes { get; private set; }

        static CableTypesRepository()
        {
            CustomCableType = new CableType() { Name = CustomCableTypeName, Resistivity = 0.05 };
            CableTypes = new List<CableType>();
        }

        public static CableType GetCableType(string name, double resistivity)
        {
            return CableTypes.Where(x => x.Name == name && x.Resistivity == resistivity).FirstOrDefault();
        }

        public static CableType GetCableType(string name)
        {
            return CableTypes.Where(x => x.Name == name).FirstOrDefault();
        }

        public static CableType GetCableType(double resistivity)
        {
            return CableTypes.Where(x => x.Resistivity == resistivity).FirstOrDefault();
        }

        public static bool SaveToFile(string fileName)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<CableType>));
                using (var streamWriter = new StreamWriter(fileName))
                {
                    xmlSerializer.Serialize(streamWriter, CableTypes.Where(x => x != CustomCableType).ToList());
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool LoadOrDefault(string fileName)
        {
            bool loaded = LoadFromFile(fileName);
            if (!loaded)
                Default();
            return loaded;
        }

        public static bool LoadFromFile(string fileName)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<CableType>));
                using (var streamReader = new StreamReader(fileName))
                {
                    var cableTypes = (List<CableType>)xmlSerializer.Deserialize(streamReader);
                    CableTypes.Clear();
                    CableTypes.AddRange(cableTypes);
                    CableTypes.RemoveAll(x => x.Name == CustomCableTypeName && x.Resistivity == 0);
                    CableTypes.Insert(0, CustomCableType);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static void Default()
        {
            Initialize();
            
            CableTypes.Add(new CableType() {
                Name = @"КПСнг(А)-FRLS 1x2x0,35",
                Resistivity = 0.04849
                });
            CableTypes.Add(new CableType()
            {
                Name = @"КПСЭнг(А)-FRLS 1x2x0,35",
                Resistivity = 0.04849
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КПСнг(А)-FRLS 1x2x0,5",
                Resistivity = 0.03401
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КПСЭнг(А)-FRLS 1x2x0,5",
                Resistivity = 0.03401
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КПСнг(А)-FRLS 1x2x0,5 ",
                Resistivity = 0.03401
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КПСЭнг(А)-FRLS 1x2x0,5",
                Resistivity = 0.03401
            });
            CableTypes.Add(new CableType()
            {
                Name = @"UT 105нг(A)-FRLS FE180 1x2x0,5",
                Resistivity = 0.03401
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КСРВнг(А)-FRLS 2x0,5 (0,2 мм²)",
                Resistivity = 0.08706
            });
            CableTypes.Add(new CableType()
            {
                Name = @"КСРЭВнг(А)-FRLS 2x0,5 (0,2 мм²)",
                Resistivity = 0.08706
            });
        }

        public static void Initialize()
        {
            CableTypes.Clear();
            CableTypes.Add(CustomCableType);
        }
	}
}
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Infrastructure.Common;

namespace FiresecAPI.Models
{
    public static class JournalManager
    {
        public static List<JournalFilter> Filters { get; set; }

        static JournalManager()
        {
            Filters = new List<JournalFilter>();
            Load();
        }

        static void Load()
        {
            using (var fileStream =
                new FileStream(PathHelper.JournalFiltersFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serializer = new XmlSerializer(typeof(JournalFiltersManager));
                var journalFiltersManager = (JournalFiltersManager) serializer.Deserialize(fileStream);
                Filters = journalFiltersManager.Filters;
            }

        }

        public static void Save()
        {
            var journalFiltersManager = new JournalFiltersManager();
            journalFiltersManager.Filters = Filters;

            using (var fileStream =
                new FileStream(PathHelper.JournalFiltersFileName, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var serializer = new XmlSerializer(typeof(JournalFiltersManager));
                serializer.Serialize(fileStream, journalFiltersManager);
            }
        }
    }
}

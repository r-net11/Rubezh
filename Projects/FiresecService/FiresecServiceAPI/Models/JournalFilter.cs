using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class JournalFilter
    {
        public JournalFilter()
        {
            Events = new List<Event>{
                new Event("Тревога"),
                new Event("Внимание"),
                new Event("Неисправность"),
                new Event("Требуется обслуживание"),
                new Event("Тревоги отключены"),
                new Event("Информация"),
                new Event("Прочие")
            };

            Categories = new List<Category>(){
                new Category("Прочие устройства"),
                new Category("Прибор"),
                new Category("Датчик"),
                new Category("Исполнительное устройство"),
                new Category("Сеть передачи данных"),
                new Category("Удаленный сервер"),
                new Category("[Без устройства]")
            };
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastRecordsCount { get; set; }

        [DataMember]
        public string LastDaysCount { get; set; }

        [DataMember]
        public List<Event> Events { get; set; }

        [DataMember]
        public List<Category> Categories { get; set; }
    }
}

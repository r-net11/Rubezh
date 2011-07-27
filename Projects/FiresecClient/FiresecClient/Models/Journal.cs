using System.Collections.Generic;

namespace FiresecClient.Models
{
    public class Journal
    {
        public Journal()
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

        public string Name { get; set; }
        public string LastRecordsCount { get; set; }
        public string LastDaysCount { get; set; }
        public List<Event> Events { get; set; }
        public List<Category> Categories { get; set; }
    }
}

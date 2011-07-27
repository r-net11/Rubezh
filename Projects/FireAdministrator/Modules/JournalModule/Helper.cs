using System.Linq;
using JournalModule.ViewModels;

namespace JournalModule
{
    public static class Helper
    {
        public static void CopyContent(JournalViewModel source, JournalViewModel dest)
        {
            dest.Name = source.Name;
            dest.LastRecordsCount = source.LastRecordsCount;
            dest.LastDaysCount = source.LastDaysCount;

            foreach (var event_ in source.Events)
            {
                dest.Events.First(x => x.Name == event_.Name).IsEnable = event_.IsEnable;
            }

            foreach (var category in source.Categories)
            {
                dest.Categories.First(x => x.Name == category.Name).IsEnable = category.IsEnable;
            }
        }
    }
}

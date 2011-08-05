using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecAPI
{
    public static class StateTypeConverter
    {
        public static string ConvertStateTypeToString(int id)
        {
            switch (id)
            {
                case 0:
                    return "Тревога";

                case 1:
                    return "Внимание (предтревожное)";

                case 2:
                    return "Неисправность";

                case 3:
                    return "Требуется обслуживание";

                case 4:
                    return "Обход устройств";

                case 5:
                    return "Неопределено";

                case 6:
                    return "Норма(*)";

                case 7:
                    return "Норма";

                default:
                    return "";
            }
        }

        public static List<string> ConvertStateTypeToListString()
        {
            List<string> availableStates = new List<string>();

            foreach (var id in Enum.GetValues(typeof(StateType)))
            {
                string str = ConvertStateTypeToString((int)id);
                if (!string.IsNullOrWhiteSpace(str))
                {
                    availableStates.Add(ConvertStateTypeToString((int)id));
                }
            }
            return availableStates;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecAPI
{
    public static class StateHelper
    {
        public static string StateTypeToString(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "Тревога";

                case StateType.Attention:
                    return "Внимание (предтревожное)";

                case StateType.Failure:
                    return "Неисправность";

                case StateType.Service:
                    return "Требуется обслуживание";

                case StateType.Off:
                    return "Обход устройств";

                case StateType.Unknown:
                    return "Неопределено";

                case StateType.Info:
                    return "Норма(*)";

                case StateType.Norm:
                    return "Норма";

                case StateType.No:
                    return "Нет состояния";

                default:
                    return "";
            }
        }
    }
}
